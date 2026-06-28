using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Availability;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly EIskeleDbContext _dbContext;

    public AvailabilityService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<AvailabilitySlotDto>>> CheckAvailabilityAsync(CheckAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var slots = await _dbContext.AvailabilitySlots
            .Where(s => s.BoatId == request.BoatId && s.StartDateTime >= request.StartDate && s.EndDateTime <= request.EndDate)
            .ToListAsync(cancellationToken);

        var response = slots.Select(s => new AvailabilitySlotDto
        {
            Id = s.Id,
            BoatId = s.BoatId,
            StartDateTime = s.StartDateTime,
            EndDateTime = s.EndDateTime,
            Status = s.Status.ToString()
        });

        return Result<IEnumerable<AvailabilitySlotDto>>.Success(response);
    }

    public async Task<Result> BlockDatesAsync(BlockDatesRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StartDate >= request.EndDate)
        {
            return Result.Failure("AVAILABILITY.INVALID_DATES", "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
        }

        var boat = await _dbContext.Boats.FindAsync(new object[] { request.BoatId }, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        // Çakışan rezervasyon kontrolü
        var hasReservations = await _dbContext.Reservations
            .Where(r => r.BoatId == request.BoatId && r.Status != Domain.Enums.ReservationStatus.Cancelled && r.Status != Domain.Enums.ReservationStatus.Rejected)
            .AnyAsync(r => request.StartDate < r.EndDateTime && request.EndDate > r.StartDateTime, cancellationToken);

        if (hasReservations)
        {
            return Result.Failure("AVAILABILITY.HAS_RESERVATIONS", "Bloklanmak istenen tarihler arasında mevcut rezervasyonlar var.");
        }

        var slot = new AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            BoatId = request.BoatId,
            StartDateTime = request.StartDate,
            EndDateTime = request.EndDate,
            Status = AvailabilitySlotStatus.Closed,
            Reason = request.Reason
        };

        _dbContext.AvailabilitySlots.Add(slot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
