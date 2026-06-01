using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class AvailabilitySlot : BaseEntity
{
    public Guid BoatId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Status { get; set; } = "Available"; // Available, Booked, Closed, Maintenance
    public string? Reason { get; set; }

    // Navigation
    public Boat Boat { get; set; } = null!;
}
