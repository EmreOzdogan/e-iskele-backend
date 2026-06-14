using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Payment : BaseEntity, IAuditableEntity
{
    public string PaymentNo { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    
    public decimal GrossTourAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public decimal ServiceFeeAmount { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal CaptainEarnings { get; set; }
    
    public PaymentProvider PaymentProvider { get; set; } = PaymentProvider.None;
    public string? ProviderReferenceNo { get; set; }
    public string? ProviderTransactionId { get; set; }
    
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PayoutStatus PayoutStatus { get; set; } = PayoutStatus.None;
    public RefundStatus RefundStatus { get; set; } = RefundStatus.None;
    
    public decimal? RefundedAmount { get; set; }
    
    public DateTime? PaidAt { get; set; }
}
