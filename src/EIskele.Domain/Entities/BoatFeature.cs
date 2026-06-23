using System;
using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class BoatFeature : BaseEntity
{
    public Guid BoatId { get; set; }
    
    // e.g. "WC", "Mutfak", "Gölgelik", "Can yeleği"
    public string Name { get; set; } = string.Empty;
    
    // Feature or SafetyEquipment
    public string Category { get; set; } = string.Empty;
    
    // Is it available according to the captain?
    public bool IsAvailable { get; set; }
    
    public BoatFeatureStatus Status { get; set; } = BoatFeatureStatus.PendingReview;
    
    public Boat Boat { get; set; } = null!;
}
