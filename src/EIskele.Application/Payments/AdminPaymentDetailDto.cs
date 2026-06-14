using System;
using System.Collections.Generic;

namespace EIskele.Application.Payments;

public class AdminPaymentDetailDto
{
    public Guid Id { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public string? TransactionId { get; set; }

    public Guid ReservationId { get; set; }
    public string ReservationNo { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhoneMasked { get; set; }

    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string? CaptainCompanyName { get; set; }

    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;

    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    public DateTime TourDate { get; set; }

    public decimal GrossTourAmount { get; set; }
    public decimal ServiceFeeAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DepositRate { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }

    public decimal PlatformCommissionRate { get; set; }
    public decimal PlatformCommissionAmount { get; set; }
    public decimal CaptainPayoutAmount { get; set; }
    public decimal? RefundedAmount { get; set; }
    public decimal NetPlatformRevenue { get; set; }

    public string Currency { get; set; } = "TRY";

    public string PaymentProvider { get; set; } = "none";
    public string? ProviderReferenceNo { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ProviderStatusCode { get; set; }
    public string? ProviderMessage { get; set; }

    public string PaymentMethod { get; set; } = "notRequired";
    public string PaymentStatus { get; set; } = "pending";
    public string PayoutStatus { get; set; } = "none";
    public string RefundStatus { get; set; } = "none";

    public bool? IsManualUpdate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // We will populate these arrays in real code if needed or keep empty
    public List<PaymentProviderLogDto> ProviderLogs { get; set; } = new();
    public PaymentRefundInfoDto? RefundInfo { get; set; }
    public PaymentPayoutInfoDto? PayoutInfo { get; set; }
    public List<PaymentAdminNoteDto> AdminNotes { get; set; } = new();
    public List<PaymentAuditLogDto> AuditLogs { get; set; } = new();
}

public class PaymentProviderLogDto
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? StatusCode { get; set; }
    public string? Message { get; set; }
    public bool WebhookReceived { get; set; }
    public bool WebhookVerified { get; set; }
    public DateTime? WebhookReceivedAt { get; set; }
    public bool IsTestMode { get; set; }
    public bool IsThreeDSecure { get; set; }
}

public class PaymentRefundInfoDto
{
    public Guid Id { get; set; }
    public string RefundStatus { get; set; } = string.Empty;
    public string? RefundType { get; set; }
    public string? RefundReason { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime? RequestedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public decimal? RefundedAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public bool CustomerNotified { get; set; }
    public bool CaptainNotified { get; set; }
}

public class PaymentPayoutInfoDto
{
    public Guid Id { get; set; }
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string IbanStatus { get; set; } = "missing";
    public string PayoutStatus { get; set; } = "pending";
    public decimal GrossTourAmount { get; set; }
    public decimal PlatformCommissionAmount { get; set; }
    public decimal CaptainPayoutAmount { get; set; }
    public decimal? BlockedAmount { get; set; }
    public decimal NetPayableAmount { get; set; }
    public DateTime? CalculatedAt { get; set; }
    public DateTime? ScheduledPaymentAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? BlockReason { get; set; }
}

public class PaymentAdminNoteDto
{
    public Guid Id { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PaymentAuditLogDto
{
    public Guid Id { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? ActorIp { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }
}
