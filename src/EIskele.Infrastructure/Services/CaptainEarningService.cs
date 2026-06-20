using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EIskele.Application.Common.Results;
using EIskele.Application.Earnings;
using EIskele.Domain.Enums;
using EIskele.Persistence;

namespace EIskele.Infrastructure.Services;

public class CaptainEarningService : ICaptainEarningService
{
    private readonly EIskeleDbContext _context;

    public CaptainEarningService(EIskeleDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CaptainEarningSummaryDto>> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var captain = await _context.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
            return Result<CaptainEarningSummaryDto>.Failure("Captain.NotFound", "Kaptan profili bulunamadı.");

        // Gerçek veriler
        var completedReservationsAmount = await _context.Reservations
            .Where(r => r.Boat.CaptainId == captain.Id && r.Status == ReservationStatus.Completed)
            .SumAsync(r => r.TotalPrice, cancellationToken);

        var pendingPayouts = await _context.Payouts
            .Where(p => p.CaptainId == captain.Id && p.Status == PayoutStatus.Pending)
            .SumAsync(p => p.Amount, cancellationToken);

        var completedPayouts = await _context.Payouts
            .Where(p => p.CaptainId == captain.Id && p.Status == PayoutStatus.Paid)
            .SumAsync(p => p.Amount, cancellationToken);
            
        var nextPayout = await _context.Payouts
            .Where(p => p.CaptainId == captain.Id && p.Status == PayoutStatus.Scheduled)
            .OrderBy(p => p.ScheduledDate)
            .FirstOrDefaultAsync(cancellationToken);

        var summary = new CaptainEarningSummaryDto
        {
            TotalEarnings = completedReservationsAmount,
            PendingPayouts = pendingPayouts,
            CompletedPayouts = completedPayouts,
            NextPayoutAmount = nextPayout?.Amount ?? 0m,
            NextPayoutDate = nextPayout?.ScheduledDate
        };

        return Result<CaptainEarningSummaryDto>.Success(summary);
    }

    public async Task<Result<IEnumerable<CaptainEarningHistoryItemDto>>> GetHistoryAsync(Guid userId, string period = "all", CancellationToken cancellationToken = default)
    {
        var captain = await _context.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
            return Result<IEnumerable<CaptainEarningHistoryItemDto>>.Failure("Captain.NotFound", "Kaptan profili bulunamadı.");

        var query = _context.Reservations
            .Include(r => r.Boat)
            .Where(r => r.Boat.CaptainId == captain.Id && 
                       (r.Status == ReservationStatus.Completed || r.Status == ReservationStatus.Paid));

        // Filter period logically if needed, omitted for brevity to match "all" default

        var reservations = await query
            .OrderByDescending(r => r.StartDateTime)
            .Select(r => new CaptainEarningHistoryItemDto
            {
                Id = r.Id,
                ReservationNo = r.ReservationNo,
                BoatName = r.Boat.Name,
                Date = r.StartDateTime,
                Amount = r.TotalPrice,
                Status = r.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<CaptainEarningHistoryItemDto>>.Success(reservations);
    }

    public async Task<Result<IEnumerable<CaptainPayoutListItemDto>>> GetPayoutsAsync(Guid userId, string status = "all", CancellationToken cancellationToken = default)
    {
        var captain = await _context.Captains
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (captain == null)
            return Result<IEnumerable<CaptainPayoutListItemDto>>.Failure("Captain.NotFound", "Kaptan profili bulunamadı.");

        var query = _context.Payouts
            .Where(p => p.CaptainId == captain.Id);

        if (!string.IsNullOrEmpty(status) && status != "all" && Enum.TryParse<PayoutStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        var payouts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new CaptainPayoutListItemDto
            {
                Id = p.Id,
                PayoutNo = p.PayoutNo,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                ScheduledDate = p.ScheduledDate,
                PaidDate = p.PaidDate,
                RelatedReservationCount = p.RelatedReservationCount
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<CaptainPayoutListItemDto>>.Success(payouts);
    }
}
