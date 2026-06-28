using System;

namespace EIskele.Application.Payouts;

public class PayoutDto
{
    public Guid Id { get; set; }
    public string PayoutNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string IbanMasked { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public int RelatedReservationCount { get; set; }
    public string? Description { get; set; }
}

public class UpdatePayoutStatusDto
{
    public string Status { get; set; } = string.Empty;
}
