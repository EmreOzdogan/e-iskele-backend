using System;
using System.Collections.Generic;

namespace EIskele.Application.Captains;

public class AdminCaptainListItemDto
{
    public Guid Id { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public int TotalBoatCount { get; set; }
    public int ActiveBoatCount { get; set; }
    public string DocumentStatus { get; set; } = string.Empty;
    public string ApplicationStatus { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int TotalReservationCount { get; set; }
    public int CompletedReservationCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetAdminCaptainsQuery
{
    public string? Search { get; set; }
    public string? ApplicationStatus { get; set; }
    public string? ApplicationType { get; set; }
    public string? DocumentStatus { get; set; }
    public string? CaptainStatus { get; set; }
    public Guid? LocationId { get; set; }
    public double? MinRating { get; set; }
    public double? MaxRating { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "desc";
}

public class AdminCaptainsSummaryDto
{
    public int TotalCaptains { get; set; }
    public int ApprovedCaptains { get; set; }
    public int PendingApplications { get; set; }
    public int MissingDocuments { get; set; }
    public int SuspendedCaptains { get; set; }
    public int CompanyApplications { get; set; }
    public int IndividualApplications { get; set; }
    public int NewApplicationsThisMonth { get; set; }
}

public class AdminCaptainDetailDto
{
    public Guid Id { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public string ApplicationStatus { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public string DocumentSummaryStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public double? AverageRating { get; set; }
    public int TotalReservations { get; set; }
    public int CompletedReservations { get; set; }
    
    public CaptainIndividualInfoDto? IndividualInfo { get; set; }
    public CaptainCompanyInfoDto? CompanyInfo { get; set; }
    
    public List<CaptainDocumentDto> Documents { get; set; } = new();
    public List<CaptainBoatSummaryDto> Boats { get; set; } = new();
    public List<CaptainAdminNoteDto> AdminNotes { get; set; } = new();
    public List<CaptainAuditLogDto> AuditLogs { get; set; } = new();
    public List<CaptainReservationSummaryDto> Reservations { get; set; } = new();
    public CaptainPerformanceMetricsDto? PerformanceMetrics { get; set; }
}

public class CaptainIndividualInfoDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? IdentityNumberMasked { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? IbanMasked { get; set; }
}

public class CaptainCompanyInfoDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string AuthorizedPersonName { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public string? TaxNumberMasked { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? IbanMasked { get; set; }
}

public class CaptainDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileSizeText { get; set; }
    public DateTime? UploadedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
}

public class CaptainBoatSummaryDto
{
    public Guid Id { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ActivePackageCount { get; set; }
    public string BoatStatus { get; set; } = string.Empty;
    public string DocumentStatus { get; set; } = string.Empty;
    public string PublishStatus { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}

public class CaptainAdminNoteDto
{
    public Guid Id { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CaptainAuditLogDto
{
    public Guid Id { get; set; }
    public string ActionDisplay { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }
}

public class CaptainReservationSummaryDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
}

public class CaptainPerformanceMetricsDto
{
    public int RevenueRank { get; set; }
    public string ResponseRateText { get; set; } = string.Empty;
    public double CancellationRate { get; set; }
    public double CompletionRate { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
