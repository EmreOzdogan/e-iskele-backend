using System;

namespace EIskele.Application.Packages;

public class AdminTourPackageListItemDto
{
    public Guid Id { get; set; }
    public string PackageNo { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string BoatPublishStatus { get; set; } = string.Empty;
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string CaptainApplicationType { get; set; } = string.Empty;
    public string CaptainStatus { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }
    public string TourType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinGuests { get; set; }
    public int MaxGuests { get; set; }
    public string DurationText { get; set; } = string.Empty;
    public string PackageStatus { get; set; } = string.Empty;
    public DateTime SubmittedForReviewAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
