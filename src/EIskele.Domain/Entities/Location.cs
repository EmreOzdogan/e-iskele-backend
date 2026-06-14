using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public LocationType Type { get; set; } = LocationType.City;
    public Guid? ParentLocationId { get; set; }
    public Location? ParentLocation { get; set; }
    public ICollection<Location> ChildLocations { get; set; } = new List<Location>();
    
    public LocationRegion Region { get; set; } = LocationRegion.Ege;
    public LocationStatus Status { get; set; } = LocationStatus.Draft;
    
    public bool IsPopular { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    
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
    
    public LocationSeoStatus SeoStatus { get; set; } = LocationSeoStatus.Missing;
    public LocationCoordinateStatus CoordinateStatus { get; set; } = LocationCoordinateStatus.Missing;
    
    // Navigation
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
    public ICollection<Harbor> Harbors { get; set; } = new List<Harbor>();
}
