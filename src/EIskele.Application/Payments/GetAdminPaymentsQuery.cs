using System;

namespace EIskele.Application.Payments;

public class GetAdminPaymentsQuery
{
    public string? Search { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PayoutStatus { get; set; }
    public string? RefundStatus { get; set; }
    public string? PaymentProvider { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? ReservationStatus { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CaptainId { get; set; }
    public Guid? BoatId { get; set; }
    public Guid? PackageId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Currency { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
