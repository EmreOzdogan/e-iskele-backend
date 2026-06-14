using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class TourPackage : SoftDeletableEntity
{
    public Guid BoatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }
    public ReservationApprovalType ApprovalType { get; set; } = ReservationApprovalType.AutoApprove;

    public string TourType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string TimeLabel { get; set; } = string.Empty;
    public decimal PrepaymentPercentage { get; set; }
    public decimal ServiceFee { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsChildFriendly { get; set; }
    
    public string CancellationPolicyType { get; set; } = string.Empty;
    public int FreeCancellationHours { get; set; }
    public string? CaptainCancellationNote { get; set; }
    public string? WeatherPostponeNote { get; set; }
    public string? RefundPolicyNote { get; set; }
    
    public TourPackageStatus Status { get; set; } = TourPackageStatus.Draft;

    // Navigation
    public Boat Boat { get; set; } = null!;
    public ICollection<PackageInclude> Includes { get; set; } = new List<PackageInclude>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
