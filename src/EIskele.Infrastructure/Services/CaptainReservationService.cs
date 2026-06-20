using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class CaptainReservationService : EIskele.Application.Reservations.ICaptainReservationService
{
    private readonly EIskeleDbContext _dbContext;

    public CaptainReservationService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<Guid?> GetCaptainIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        return captain?.Id;
    }

    public async Task<Result<IEnumerable<EIskele.Application.Reservations.CaptainReservationListItemDto>>> GetReservationsAsync(Guid userId, string status = "all", CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result<IEnumerable<EIskele.Application.Reservations.CaptainReservationListItemDto>>.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var boatIds = await _dbContext.Boats.Where(b => b.CaptainId == captainId.Value).Select(b => b.Id).ToListAsync(cancellationToken);

        var query = _dbContext.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .Include(r => r.TourPackage)
            .Where(r => boatIds.Contains(r.BoatId));

        if (status != "all")
        {
            if (Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(r => r.Status == parsedStatus);
            }
        }

        var list = await query
            .OrderByDescending(r => r.StartDateTime)
            .Select(r => new EIskele.Application.Reservations.CaptainReservationListItemDto
            {
                Id = r.Id,
                ReservationNo = r.ReservationNo,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                BoatId = r.BoatId,
                BoatName = r.Boat.Name,
                PackageId = r.TourPackageId,
                PackageName = r.TourPackage.Name,
                Date = r.StartDateTime.ToString("yyyy-MM-dd"),
                Time = $"{r.StartDateTime:HH:mm} - {r.EndDateTime:HH:mm}",
                GuestCount = r.GuestCount,
                TotalPrice = r.TotalPrice,
                Status = r.Status.ToString(),
                PaymentStatus = "Pending" // Assuming pending for MVP without payment module
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<EIskele.Application.Reservations.CaptainReservationListItemDto>>.Success(list);
    }

    public async Task<Result<EIskele.Application.Reservations.CaptainReservationDetailDto>> GetReservationDetailAsync(Guid userId, Guid reservationId, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result<EIskele.Application.Reservations.CaptainReservationDetailDto>.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var reservation = await _dbContext.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Boat)
            .Include(r => r.TourPackage)
            .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);

        if (reservation == null) return Result<EIskele.Application.Reservations.CaptainReservationDetailDto>.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        if (reservation.Boat.CaptainId != captainId.Value) return Result<EIskele.Application.Reservations.CaptainReservationDetailDto>.Failure("FORBIDDEN", "Yetkiniz yok.");

        var dto = new EIskele.Application.Reservations.CaptainReservationDetailDto
        {
            Id = reservation.Id,
            ReservationNo = reservation.ReservationNo,
            CustomerName = reservation.Customer.FirstName + " " + reservation.Customer.LastName,
            CustomerEmail = reservation.Customer.Email ?? "",
            CustomerPhone = reservation.Customer.PhoneNumber ?? "",
            CustomerNote = "",
            CaptainNote = reservation.CaptainNote,
            BoatId = reservation.BoatId,
            BoatName = reservation.Boat.Name,
            PackageId = reservation.TourPackageId,
            PackageName = reservation.TourPackage.Name,
            Date = reservation.StartDateTime.ToString("yyyy-MM-dd"),
            Time = $"{reservation.StartDateTime:HH:mm} - {reservation.EndDateTime:HH:mm}",
            GuestCount = reservation.GuestCount,
            TotalPrice = reservation.TotalPrice,
            PricePerPerson = reservation.GuestCount > 0 ? reservation.TotalPrice / reservation.GuestCount : 0,
            CancellationPolicy = reservation.TourPackage.CancellationPolicyType,
            Status = reservation.Status.ToString(),
            PaymentStatus = "Pending",
            CreatedAt = reservation.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        };

        return Result<EIskele.Application.Reservations.CaptainReservationDetailDto>.Success(dto);
    }

    public async Task<Result> ApproveReservationAsync(Guid userId, Guid reservationId, string? note, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var reservation = await _dbContext.Reservations.Include(r => r.Boat).FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
        if (reservation == null) return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        if (reservation.Boat.CaptainId != captainId.Value) return Result.Failure("FORBIDDEN", "Yetkiniz yok.");

        if (reservation.Status != ReservationStatus.WaitingCaptainApproval)
        {
            return Result.Failure("BUSINESS_RULE", "Bu rezervasyon onaylanabilecek durumda değil.");
        }

        reservation.Status = ReservationStatus.Approved;
        if (!string.IsNullOrEmpty(note))
        {
            reservation.CaptainNote = note;
        }
        
        // Slot is automatically booked when created, so we don't necessarily have to create an AvailabilitySlot here for MVP if it's handled by queries.

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RejectReservationAsync(Guid userId, Guid reservationId, string reason, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var reservation = await _dbContext.Reservations.Include(r => r.Boat).FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
        if (reservation == null) return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        if (reservation.Boat.CaptainId != captainId.Value) return Result.Failure("FORBIDDEN", "Yetkiniz yok.");

        if (reservation.Status != ReservationStatus.WaitingCaptainApproval && reservation.Status != ReservationStatus.Pending)
        {
            return Result.Failure("BUSINESS_RULE", "Bu rezervasyon iptal/red edilebilecek durumda değil.");
        }

        reservation.Status = ReservationStatus.Rejected;
        reservation.CaptainNote = string.IsNullOrEmpty(reservation.CaptainNote) ? reason : reservation.CaptainNote + " | Ret Nedeni: " + reason;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelReservationAsync(Guid userId, Guid reservationId, string reason, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var reservation = await _dbContext.Reservations.Include(r => r.Boat).FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
        if (reservation == null) return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        if (reservation.Boat.CaptainId != captainId.Value) return Result.Failure("FORBIDDEN", "Yetkiniz yok.");

        if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
        {
            return Result.Failure("BUSINESS_RULE", "Bu rezervasyon zaten iptal edilmiş veya tamamlanmış.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CaptainNote = string.IsNullOrEmpty(reservation.CaptainNote) ? reason : reservation.CaptainNote + " | İptal Nedeni: " + reason;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateCaptainNoteAsync(Guid userId, Guid reservationId, string note, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var reservation = await _dbContext.Reservations.Include(r => r.Boat).FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
        if (reservation == null) return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
        if (reservation.Boat.CaptainId != captainId.Value) return Result.Failure("FORBIDDEN", "Yetkiniz yok.");

        reservation.CaptainNote = note;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
