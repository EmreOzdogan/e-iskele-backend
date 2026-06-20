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
using EIskele.Application.Availability;

namespace EIskele.Infrastructure.Services;

public class CaptainCalendarService : ICaptainCalendarService
{
    private readonly EIskeleDbContext _dbContext;

    public CaptainCalendarService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<Guid?> GetCaptainIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var captain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        return captain?.Id;
    }

    public async Task<Result<CaptainCalendarMetricsDto>> GetMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue)
        {
            return Result<CaptainCalendarMetricsDto>.Failure("NOT_FOUND", "Kaptan bulunamadı.");
        }

        var boatIds = await _dbContext.Boats
            .Where(b => b.CaptainId == captainId.Value)
            .Select(b => b.Id)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

        var slots = await _dbContext.AvailabilitySlots
            .Where(s => boatIds.Contains(s.BoatId) && s.StartDateTime >= startOfMonth && s.StartDateTime <= endOfMonth)
            .ToListAsync(cancellationToken);

        var reservations = await _dbContext.Reservations
            .Where(r => boatIds.Contains(r.BoatId) && r.StartDateTime >= startOfMonth && r.StartDateTime <= endOfMonth)
            .ToListAsync(cancellationToken);

        var metrics = new CaptainCalendarMetricsDto
        {
            AvailableDaysThisMonth = 30, // Simplified for MVP. Actually we should count unblocked days.
            BookedDaysThisMonth = reservations.Count(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Paid || r.Status == ReservationStatus.Completed),
            ClosedDays = slots.Count(s => s.Status == "Closed"),
            MaintenanceDays = slots.Count(s => s.Status == "Maintenance"),
            PendingReservations = reservations.Count(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.WaitingCaptainApproval),
            LastUpdatedAt = DateTime.UtcNow.ToString("O")
        };

        return Result<CaptainCalendarMetricsDto>.Success(metrics);
    }

    public async Task<Result<IEnumerable<CaptainCalendarSlotDto>>> GetSlotsAsync(Guid userId, CaptainCalendarFilterDto filters, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue)
        {
            return Result<IEnumerable<CaptainCalendarSlotDto>>.Failure("NOT_FOUND", "Kaptan bulunamadı.");
        }

        var boatIdsQuery = _dbContext.Boats.Where(b => b.CaptainId == captainId.Value);
        
        if (filters.BoatId.HasValue)
        {
            boatIdsQuery = boatIdsQuery.Where(b => b.Id == filters.BoatId.Value);
        }

        var boatIds = await boatIdsQuery.Select(b => b.Id).ToListAsync(cancellationToken);

        var startQuery = filters.Date.Date;
        var endQuery = filters.View == "month" ? startQuery.AddMonths(1) : startQuery.AddDays(7); // Basic range handling

        var availabilitySlots = await _dbContext.AvailabilitySlots
            .Include(s => s.Boat)
            .Include(s => s.TourPackage)
            .Where(s => boatIds.Contains(s.BoatId) && s.StartDateTime >= startQuery && s.StartDateTime < endQuery)
            .ToListAsync(cancellationToken);

        var reservations = await _dbContext.Reservations
            .Include(r => r.Boat)
            .Include(r => r.TourPackage)
            .Where(r => boatIds.Contains(r.BoatId) && r.StartDateTime >= startQuery && r.StartDateTime < endQuery)
            .ToListAsync(cancellationToken);

        var resultSlots = new List<CaptainCalendarSlotDto>();

        foreach (var slot in availabilitySlots)
        {
            if (filters.PackageId.HasValue && slot.TourPackageId != filters.PackageId.Value) continue;
            
            resultSlots.Add(new CaptainCalendarSlotDto
            {
                Id = slot.Id,
                BoatId = slot.BoatId,
                PackageId = slot.TourPackageId,
                Date = slot.StartDateTime.ToString("yyyy-MM-dd"),
                StartTime = slot.StartDateTime.ToString("HH:mm"),
                EndTime = slot.EndDateTime.ToString("HH:mm"),
                Status = slot.Status.ToLower(), // closed, maintenance, etc.
                Capacity = slot.Capacity ?? slot.Boat.Capacity,
                RemainingCapacity = slot.Capacity ?? slot.Boat.Capacity, // In real scenario, subtract active reservations
                Note = slot.Reason,
                BoatName = slot.Boat.Name,
                PackageName = slot.TourPackage?.Name
            });
        }

        foreach (var res in reservations)
        {
            if (filters.PackageId.HasValue && res.TourPackageId != filters.PackageId.Value) continue;

            string status = "pending";
            if (res.Status == ReservationStatus.Approved || res.Status == ReservationStatus.Paid) status = "booked";
            else if (res.Status == ReservationStatus.Cancelled || res.Status == ReservationStatus.Rejected) continue; // skip cancelled in calendar

            resultSlots.Add(new CaptainCalendarSlotDto
            {
                Id = res.Id,
                BoatId = res.BoatId,
                PackageId = res.TourPackageId,
                Date = res.StartDateTime.ToString("yyyy-MM-dd"),
                StartTime = res.StartDateTime.ToString("HH:mm"),
                EndTime = res.EndDateTime.ToString("HH:mm"),
                Status = status,
                Capacity = res.GuestCount,
                RemainingCapacity = 0,
                Note = "Rezervasyon: " + res.ReservationNo,
                BoatName = res.Boat.Name,
                PackageName = res.TourPackage.Name,
                ReservationsCount = 1
            });
        }

        if (filters.Status != "all")
        {
            resultSlots = resultSlots.Where(s => s.Status.Equals(filters.Status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Result<IEnumerable<CaptainCalendarSlotDto>>.Success(resultSlots);
    }

    public async Task<Result> AddAvailabilityBlockAsync(Guid userId, AddAvailabilityBlockRequest request, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue)
        {
            return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");
        }

        var boat = await _dbContext.Boats.FirstOrDefaultAsync(b => b.Id == request.BoatId && b.CaptainId == captainId.Value, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı veya yetkiniz yok.");
        }

        if (!TimeSpan.TryParse(request.StartTime, out var startTime) || !TimeSpan.TryParse(request.EndTime, out var endTime))
        {
            return Result.Failure("VALIDATION_ERROR", "Saat formatı geçersiz.");
        }

        var startDateTime = request.SelectedDate.Date.Add(startTime);
        var endDateTime = request.SelectedDate.Date.Add(endTime);

        if (startDateTime >= endDateTime)
        {
            return Result.Failure("VALIDATION_ERROR", "Bitiş saati başlangıç saatinden sonra olmalıdır.");
        }

        var slot = new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            BoatId = request.BoatId,
            TourPackageId = request.PackageId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Status = "Closed", // Default or determine from note/type
            Reason = request.Note,
            Capacity = request.Capacity
        };

        _dbContext.AvailabilitySlots.Add(slot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAvailabilityBlockAsync(Guid userId, Guid slotId, CancellationToken cancellationToken = default)
    {
        var captainId = await GetCaptainIdAsync(userId, cancellationToken);
        if (!captainId.HasValue) return Result.Failure("NOT_FOUND", "Kaptan bulunamadı.");

        var slot = await _dbContext.AvailabilitySlots.Include(s => s.Boat).FirstOrDefaultAsync(s => s.Id == slotId, cancellationToken);
        if (slot == null) return Result.Failure("NOT_FOUND", "Slot bulunamadı.");

        if (slot.Boat.CaptainId != captainId.Value) return Result.Failure("FORBIDDEN", "Yetkiniz yok.");

        _dbContext.AvailabilitySlots.Remove(slot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
