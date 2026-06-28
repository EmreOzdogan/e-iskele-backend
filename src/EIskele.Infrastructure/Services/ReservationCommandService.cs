using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Data;
using EIskele.Application.Common.Results;
using EIskele.Application.Reservations;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class ReservationCommandService : IReservationCommandService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCommandService(
        EIskeleDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var boat = await _dbContext.Boats.FindAsync(new object[] { request.BoatId }, cancellationToken);
            var package = await _dbContext.TourPackages.FindAsync(new object[] { request.TourPackageId }, cancellationToken);

            if (boat == null || package == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReservationResponse>.Failure("NOT_FOUND", "Tekne veya paket bulunamadı.");
            }

            if (request.GuestCount < package.MinCapacity || request.GuestCount > package.MaxCapacity)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReservationResponse>.Failure("RESERVATION.INVALID_GUEST_COUNT", $"Kişi sayısı {package.MinCapacity} ile {package.MaxCapacity} arasında olmalıdır.");
            }

            // Çakışma Kontrolü - Diğer rezervasyonlar
            var hasConflict = await _dbContext.Reservations
                .Where(r => r.BoatId == request.BoatId && r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Rejected)
                .AnyAsync(r => request.StartDateTime < r.EndDateTime && request.EndDateTime > r.StartDateTime, cancellationToken);

            if (hasConflict)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReservationResponse>.Failure("RESERVATION.CONFLICT", "Seçilen tarih ve saat aralığında bu tekne için başka bir rezervasyon bulunmaktadır.");
            }

            // Çakışma Kontrolü - AvailabilitySlots (Kapalı veya Bakımda)
            var hasClosedSlot = await _dbContext.AvailabilitySlots
                .Where(s => s.BoatId == request.BoatId && (s.Status == AvailabilitySlotStatus.Closed || s.Status == AvailabilitySlotStatus.Maintenance))
                .AnyAsync(s => request.StartDateTime < s.EndDateTime && request.EndDateTime > s.StartDateTime, cancellationToken);

            if (hasClosedSlot)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReservationResponse>.Failure("RESERVATION.SLOT_UNAVAILABLE", "Seçilen tarih ve saat aralığında bu tekne müsait değildir (kapalı veya bakımda).");
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
            await _unitOfWork.CommitTransactionAsync(cancellationToken); // calls SaveChanges internally

            return Result<ReservationResponse>.Success(new ReservationResponse
            {
                Id = reservation.Id,
                Status = reservation.Status.ToString()
            });
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> ApproveReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var reservation = await _dbContext.Reservations.FindAsync(new object[] { reservationId }, cancellationToken);
            if (reservation == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
            }

            if (reservation.Status != ReservationStatus.WaitingCaptainApproval && reservation.Status != ReservationStatus.Pending)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("RESERVATION.INVALID_STATUS", "Bu rezervasyon onaylanmaya uygun durumda değil.");
            }

            reservation.Status = ReservationStatus.Approved;
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> CancelReservationAsync(Guid reservationId, string reason, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var reservation = await _dbContext.Reservations.FindAsync(new object[] { reservationId }, cancellationToken);
            if (reservation == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
            }

            if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("RESERVATION.INVALID_STATUS", "Tamamlanmış veya zaten iptal edilmiş rezervasyon tekrar iptal edilemez.");
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
