using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Results;

namespace EIskele.Application.Earnings;

public class CaptainEarningSummaryDto
{
    public decimal TotalEarnings { get; set; }
    public decimal PendingPayouts { get; set; }
    public decimal CompletedPayouts { get; set; }
    public decimal NextPayoutAmount { get; set; }
    public DateTime? NextPayoutDate { get; set; }
}

public class CaptainEarningHistoryItemDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = null!;
    public string BoatName { get; set; } = null!;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
}

public class CaptainPayoutListItemDto
{
    public Guid Id { get; set; }
    public string PayoutNo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public int RelatedReservationCount { get; set; }
}

public interface ICaptainEarningService
{
    Task<Result<CaptainEarningSummaryDto>> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CaptainEarningHistoryItemDto>>> GetHistoryAsync(Guid userId, string period = "all", CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CaptainPayoutListItemDto>>> GetPayoutsAsync(Guid userId, string status = "all", CancellationToken cancellationToken = default);
}
