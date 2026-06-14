using System;

namespace EIskele.Application.Boats;

public class AdminBoatListItemDto
{
    public Guid Id { get; set; }
    public string BoatNo { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? LicenseNoMasked { get; set; }
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string CaptainApplicationType { get; set; } = "individual";
    public string CaptainStatus { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public int Capacity { get; set; }
    public int TotalPackageCount { get; set; }
    public int ActivePackageCount { get; set; }
    public string DocumentStatus { get; set; } = string.Empty;
    public string ReviewStatus { get; set; } = string.Empty;
    public string PublishStatus { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}

public class AdminBoatsSummaryDto
{
    public int TotalBoats { get; set; }
    public int PublishedBoats { get; set; }
    public int PendingReviewBoats { get; set; }
    public int RevisionRequestedBoats { get; set; }
    public int RejectedBoats { get; set; }
    public int PassiveBoats { get; set; }
    public int SuspendedBoats { get; set; }
    public int MissingDocumentBoats { get; set; }
}
