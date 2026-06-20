using System;
using System.Collections.Generic;
using EIskele.Domain.Enums;

namespace EIskele.Application.Packages;

public class CaptainPackageListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string BoatPublishStatus { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ApprovalType { get; set; } = string.Empty;
    public string DurationText { get; set; } = string.Empty;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int MinGuests { get; set; }
    public int MaxGuests { get; set; }
    public string PriceType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? DepositRate { get; set; }
    public List<string> IncludedServices { get; set; } = new();
    public int UpcomingReservationCount { get; set; }
    public string LastUpdatedText { get; set; } = string.Empty;
    public List<string>? MissingItems { get; set; }
}

public class CaptainPackageDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public string BoatPublishStatus { get; set; } = string.Empty;
    public int BoatCapacity { get; set; }
    public string BoatLocation { get; set; } = string.Empty;

    public string TourType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ApprovalType { get; set; } = string.Empty;
    public string ReservationModel { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public string Duration { get; set; } = string.Empty;
    public string? CustomDuration { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MinGuests { get; set; }
    public int MaxGuests { get; set; }

    public string PriceType { get; set; } = string.Empty;
    public decimal? PerPersonPrice { get; set; }
    public decimal? WholeBoatPrice { get; set; }
    public bool HasDeposit { get; set; }
    public decimal? DepositRate { get; set; }
    public bool UseSeasonalPricing { get; set; }

    public List<string> IncludedServices { get; set; } = new();
    public List<string> CustomIncludedServices { get; set; } = new();
    public List<string> ExcludedServices { get; set; } = new();
    public List<string> CustomExcludedServices { get; set; } = new();

    public string CancellationPolicy { get; set; } = string.Empty;
    public string? CustomCancellationPolicy { get; set; }
    public string? WeatherNote { get; set; }
    public string? SpecialRules { get; set; }

    public int UpcomingReservationCount { get; set; }
    public int CompletedReservationCount { get; set; }
    public string LastUpdatedText { get; set; } = string.Empty;
    public List<string> MissingItems { get; set; } = new();
    public List<PackageHistoryItemDto> History { get; set; } = new();
}

public class PackageHistoryItemDto
{
    public string Id { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
}

public class CreateCaptainPackageRequest
{
    public Guid BoatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public string Duration { get; set; } = string.Empty;
    public string? CustomDuration { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int MinGuests { get; set; }
    public int MaxGuests { get; set; }
    public string ReservationModel { get; set; } = string.Empty;

    public string PriceType { get; set; } = string.Empty;
    public decimal? PerPersonPrice { get; set; }
    public decimal? WholeBoatPrice { get; set; }
    public bool HasDeposit { get; set; }
    public decimal? DepositRate { get; set; }
    public bool UseSeasonalPricing { get; set; }

    public List<string> IncludedServices { get; set; } = new();
    public List<string> CustomIncludedServices { get; set; } = new();
    public List<string> ExcludedServices { get; set; } = new();
    public List<string> CustomExcludedServices { get; set; } = new();

    public string CancellationPolicy { get; set; } = string.Empty;
    public string? CustomCancellationPolicy { get; set; }
    public string ApprovalType { get; set; } = string.Empty;
    public string? WeatherNote { get; set; }
    public string? SpecialRules { get; set; }
}

public class UpdateCaptainPackageRequest : CreateCaptainPackageRequest
{
}
