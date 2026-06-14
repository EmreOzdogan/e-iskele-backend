using System;
using EIskele.Application.Common.Results;
using EIskele.Domain.Enums;

namespace EIskele.Application.Payments;

public class AdminPaymentListItemDto
{
    public Guid Id { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public Guid ReservationId { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime TourDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public decimal PlatformCommissionAmount { get; set; }
    public decimal ServiceFeeAmount { get; set; }
    public decimal CaptainPayoutAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string PaymentProvider { get; set; } = "none";
    public string? ProviderReferenceNo { get; set; }
    public string PaymentMethod { get; set; } = "notRequired";
    public string PaymentStatus { get; set; } = "pending";
    public string PayoutStatus { get; set; } = "none";
    public string? RefundStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
