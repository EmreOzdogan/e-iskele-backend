using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class AvailabilitySlot : BaseEntity
{
    public Guid BoatId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public AvailabilitySlotStatus Status { get; set; } = AvailabilitySlotStatus.Available;
    public string? Reason { get; set; }

    // Navigation
    public Boat Boat { get; set; } = null!;
    public Guid? TourPackageId { get; set; }
    public TourPackage? TourPackage { get; set; }
    
    // Override properties
    public int? Capacity { get; set; }
}
