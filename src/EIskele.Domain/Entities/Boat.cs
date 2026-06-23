using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Boat : SoftDeletableEntity
{
    public Guid CaptainId { get; set; }
    public Guid LocationId { get; set; }
    public Guid? HarborId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public BoatStatus Status { get; set; } = BoatStatus.Draft;
    
    public string BoatType { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string ProductionYear { get; set; } = string.Empty;
    public string Length { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation
    public Captain Captain { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public Harbor? Harbor { get; set; }
    public ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();
    public ICollection<BoatFeature> BoatFeatures { get; set; } = new List<BoatFeature>();
}
