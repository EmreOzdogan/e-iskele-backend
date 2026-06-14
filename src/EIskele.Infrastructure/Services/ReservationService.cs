using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Application.Reservations;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EIskele.Infrastructure.Services;

public partial class ReservationService : IReservationService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly EIskele.Application.Common.Settings.ISettingsService _settingsService;
    private readonly EIskele.Infrastructure.Emails.Services.IEmailSender _emailSender;

    public ReservationService(
        EIskeleDbContext dbContext,
        EIskele.Application.Common.Settings.ISettingsService settingsService,
        EIskele.Infrastructure.Emails.Services.IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _settingsService = settingsService;
        _emailSender = emailSender;
    }

    public async Task<Result<ReservationResponse>> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StartDateTime < DateTime.UtcNow)
        {
            return Result<ReservationResponse>.Failure("RESERVATION.INVALID_DATE", "Geçmiş tarihe rezervasyon yapılamaz.");
        }

        if (request.StartDateTime >= request.EndDateTime)
        {
            return Result<ReservationResponse>.Failure("RESERVATION.INVALID_DATE_RANGE", "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
        }

        var boat = await _dbContext.Boats.FindAsync(new object[] { request.BoatId }, cancellationToken);
        var package = await _dbContext.TourPackages.FindAsync(new object[] { request.TourPackageId }, cancellationToken);

        if (boat == null || package == null)
        {
            return Result<ReservationResponse>.Failure("NOT_FOUND", "Tekne veya paket bulunamadı.");
        }

        if (request.GuestCount < package.MinCapacity || request.GuestCount > package.MaxCapacity)
        {
            return Result<ReservationResponse>.Failure("RESERVATION.INVALID_GUEST_COUNT", $"Kişi sayısı {package.MinCapacity} ile {package.MaxCapacity} arasında olmalıdır.");
        }

        // Çakışma Kontrolü
        var hasConflict = await _dbContext.Reservations
            .Where(r => r.BoatId == request.BoatId && r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Rejected)
            .AnyAsync(r => request.StartDateTime < r.EndDateTime && request.EndDateTime > r.StartDateTime, cancellationToken);

        if (hasConflict)
        {
            return Result<ReservationResponse>.Failure("RESERVATION.CONFLICT", "Seçilen tarih ve saat aralığında bu tekne için başka bir rezervasyon bulunmaktadır.");
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            BoatId = request.BoatId,
            TourPackageId = request.TourPackageId,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            GuestCount = request.GuestCount,
            TotalPrice = package.Price, // Snapshot
            PackageNameSnapshot = package.Name,
            Status = package.ApprovalType == ReservationApprovalType.AutoApprove ? ReservationStatus.Approved : ReservationStatus.WaitingCaptainApproval
        };

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ReservationResponse>.Success(new ReservationResponse
        {
            Id = reservation.Id,
            Status = reservation.Status.ToString()
        });
    }

    public async Task<Result> ApproveReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.Reservations.FindAsync(new object[] { reservationId }, cancellationToken);
        if (reservation == null)
        {
            return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        }

        if (reservation.Status != ReservationStatus.WaitingCaptainApproval && reservation.Status != ReservationStatus.Pending)
        {
            return Result.Failure("RESERVATION.INVALID_STATUS", "Bu rezervasyon onaylanmaya uygun durumda değil.");
        }

        reservation.Status = ReservationStatus.Approved;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CancelReservationAsync(Guid reservationId, string reason, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.Reservations.FindAsync(new object[] { reservationId }, cancellationToken);
        if (reservation == null)
        {
            return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        }

        if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
        {
            return Result.Failure("RESERVATION.INVALID_STATUS", "Tamamlanmış veya zaten iptal edilmiş rezervasyon tekrar iptal edilemez.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public Task<Result<IEnumerable<ReservationResponse>>> GetReservationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<IEnumerable<ReservationResponse>>.Failure("NOT_IMPLEMENTED", "Metod henüz kodlanmadı."));
    }
}
