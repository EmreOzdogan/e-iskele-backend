using EIskele.Domain.Enums;

namespace EIskele.Application.Common.Locations;

public class AdminLocationListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int HarborCount { get; set; }
    public string? MainHarborName { get; set; }
    public int TotalBoatCount { get; set; }
    public int ActiveBoatCount { get; set; }
    public int TotalPackageCount { get; set; }
    public int ActivePackageCount { get; set; }
    public int TotalReservationCount { get; set; }
    public int MonthlyReservationCount { get; set; }
    public string SeoStatus { get; set; } = string.Empty;
    public string CoordinateStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AdminLocationsSummaryDto
{
    public int TotalLocations { get; set; }
    public int ActiveLocations { get; set; }
    public int PopularLocations { get; set; }
    public int TotalHarbors { get; set; }
    public int SeoReadyLocations { get; set; }
    public int MissingCoordinates { get; set; }
    public int MissingCoverImages { get; set; }
    public int PassiveLocations { get; set; }
}

public class AdminLocationDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public Guid? ParentLocationId { get; set; }
    public string? ParentLocationName { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
    public int? SortOrder { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? SeoTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string SeoStatus { get; set; } = string.Empty;
    public string CoordinateStatus { get; set; } = string.Empty;
    public int HarborCount { get; set; }
    public int ActiveBoatCount { get; set; }
    public int ActivePackageCount { get; set; }
    public int MonthlyReservationCount { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLocationDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public LocationType Type { get; set; }
    public LocationRegion Region { get; set; }
    public LocationStatus Status { get; set; }
    public Guid? ParentLocationId { get; set; }
    public bool IsPopular { get; set; }
    public string? Description { get; set; }
}

public class UpdateLocationDto : CreateLocationDto
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    public string? ShortDescription { get; set; }
    public string? SeoTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class DeactivateLocationDto
{
    public string? Reason { get; set; }
}

public class MarkLocationPopularDto
{
    public string? Note { get; set; }
    public int? Order { get; set; }
}

public class LocationHarborListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CoordinateStatus { get; set; } = string.Empty;
    public int ActiveBoatCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsMainDeparturePoint { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateHarborDto
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public HarborType Type { get; set; }
    public LocationStatus Status { get; set; }
    public bool IsMainDeparturePoint { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class UpdateHarborDto : CreateHarborDto
{
    public Guid Id { get; set; }
}
