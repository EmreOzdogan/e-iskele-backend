using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation
    public ICollection<Boat> Boats { get; set; } = new List<Boat>();
}
