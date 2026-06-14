using System;
using System.Collections.Generic;

namespace EIskele.Application.Boats;

public class BoatDetailDto
{
    public Guid Id { get; set; }
    public string BoatNo { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public int? ProductionYear { get; set; }
    public decimal? Length { get; set; }
    public int Capacity { get; set; }
    public string? LicenseNo { get; set; }
    public string? Description { get; set; }

    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string CaptainApplicationType { get; set; } = string.Empty;
    public string CaptainStatus { get; set; } = string.Empty;
    public string CaptainPhone { get; set; } = string.Empty;
    public string CaptainEmail { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    public string? Harbor { get; set; }

    public int ActivePackageCount { get; set; }
    public int TotalPackageCount { get; set; }

    public string BoatStatus { get; set; } = string.Empty;
    public string PublishStatus { get; set; } = string.Empty;
    public string ReviewStatus { get; set; } = string.Empty;
    public string DocumentStatus { get; set; } = string.Empty;

    public DateTime? UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class BoatImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageType { get; set; } = string.Empty; // "cover" or "gallery"
    public string Status { get; set; } = string.Empty; // "approved", "rejected", "pending", "low_quality"
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class BoatDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty; // "ruhsat", "sigorta", vb.
    public string DocumentName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "approved", "rejected", "pending", "missing", "expiring"
    public DateTime UploadedAt { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
}

public class BoatFeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "feature" or "safety"
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BoatPackageSummaryDto
{
    public Guid Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public decimal Price { get; set; }
    public string ApprovalType { get; set; } = string.Empty; // "AutoApprove" or "CaptainApprovalRequired"
    public string Status { get; set; } = string.Empty;
}
