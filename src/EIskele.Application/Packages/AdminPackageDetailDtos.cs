using System;
using System.Collections.Generic;
using EIskele.Domain.Enums;

namespace EIskele.Application.Packages;

public class PackageDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string TimeLabel { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal PrepaymentPercentage { get; set; }
    public decimal ServiceFee { get; set; }
    public string Currency { get; set; } = "TRY";

    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsChildFriendly { get; set; }

    public string CancellationPolicyType { get; set; } = string.Empty;
    public int FreeCancellationHours { get; set; }
    public string? CaptainCancellationNote { get; set; }
    public string? WeatherPostponeNote { get; set; }
    public string? RefundPolicyNote { get; set; }

    public TourPackageStatus Status { get; set; }
    public ReservationApprovalType ApprovalType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Boat Summary
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public int BoatCapacity { get; set; }
    public string BoatStatus { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Harbor { get; set; } = string.Empty;

    // Captain Summary
    public Guid CaptainId { get; set; }
    public string CaptainName { get; set; } = string.Empty;
    public string CaptainStatus { get; set; } = string.Empty;

    public List<PackageIncludeDto> Includes { get; set; } = new();
    public List<PackageReservationDto> RecentReservations { get; set; } = new();
}

public class PackageIncludeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIncluded { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PackageReservationDto
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}
