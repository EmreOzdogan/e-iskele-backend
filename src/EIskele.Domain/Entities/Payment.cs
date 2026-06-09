using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Payment : BaseEntity, IAuditableEntity
{
    public string PaymentNo { get; set; } = string.Empty;
    
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal CaptainEarnings { get; set; }
    
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public DateTime? PaidAt { get; set; }
    public string? ProviderTransactionId { get; set; }
    
}
