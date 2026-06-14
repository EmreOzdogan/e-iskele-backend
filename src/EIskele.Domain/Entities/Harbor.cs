using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Harbor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;
    
    public HarborType Type { get; set; } = HarborType.Harbor;
    public LocationStatus Status { get; set; } = LocationStatus.Active;
    
    public bool IsMainDeparturePoint { get; set; } = false;
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public LocationCoordinateStatus CoordinateStatus { get; set; } = LocationCoordinateStatus.Missing;
    
    // Navigation
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
}
