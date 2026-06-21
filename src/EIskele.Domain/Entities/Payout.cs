using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Payout : BaseEntity, IAuditableEntity
{
    public string PayoutNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string IbanMasked { get; set; } = string.Empty;
    public PayoutStatus Status { get; set; } = PayoutStatus.None;
    
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PaidDate { get; set; }
    
    public Guid CaptainId { get; set; }
    public Captain Captain { get; set; } = null!;
    
    public int RelatedReservationCount { get; set; }
    public string? Description { get; set; }

}
