using System;

namespace EIskele.Application.Reservations;

public class GetAdminReservationsQuery
{
    public string? Search { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PayoutStatus { get; set; }
    public string? RefundStatus { get; set; }
    public string? ReservationStatus { get; set; }
    public string? PaymentProvider { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CaptainId { get; set; }
    public Guid? BoatId { get; set; }
    public Guid? PackageId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}

public class AdminReservationSummaryMetricsDto
{
    public int TotalCount { get; set; }
    public int TodayCount { get; set; }
    public int WaitingCaptainCount { get; set; }
    public int PaymentPendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int PostponedCount { get; set; }
}

public class AdminReservationListItemDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public string Source { get; set; } = "web";
    public DateTime CreatedAt { get; set; }
    
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string BoatPublishStatus { get; set; } = "published";
    
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string TourType { get; set; } = "Balık Turu";
    
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    
    public DateTime TourDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int GuestCount { get; set; }
    
    public decimal TotalAmount { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    
    public string ApprovalType { get; set; } = "captainApproval";
    public string PaymentStatus { get; set; } = string.Empty;
    public string ReservationStatus { get; set; } = string.Empty;
}

public class AdminReservationDetailDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public string Source { get; set; } = "web";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }

    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;

    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string BoatPublishStatus { get; set; } = "published";

    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string TourType { get; set; } = "Balık Turu";

    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }

    public DateTime TourDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string? DurationText { get; set; }
    public int GuestCount { get; set; }

    public string? CustomerNote { get; set; }
    public string? SpecialRequest { get; set; }
    public string? HealthNote { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal? ServiceFee { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }

    public string ApprovalType { get; set; } = "captainApproval";
    public string PaymentStatus { get; set; } = string.Empty;
    public string ReservationStatus { get; set; } = string.Empty;
    
    public string? CaptainResponseStatus { get; set; }
    public string? CalendarSlotStatus { get; set; }

    public ReservationPaymentInfoDto? PaymentInfo { get; set; }
    public ReservationCancellationInfoDto? CancellationInfo { get; set; }
}

public class ReservationPaymentInfoDto
{
    public Guid? PaymentId { get; set; }
    public string? PaymentNo { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? ServiceFee { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DepositRate { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public string? ProviderReferenceNo { get; set; }
}

public class ReservationCancellationInfoDto
{
    public bool IsCancelled { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? RefundStatus { get; set; }
    public string? AdminNote { get; set; }
}
