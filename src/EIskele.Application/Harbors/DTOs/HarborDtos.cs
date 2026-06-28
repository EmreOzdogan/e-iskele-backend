using EIskele.Domain.Enums;

namespace EIskele.Application.Harbors.DTOs;

public class HarborDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty; // Useful for UI
    public HarborType Type { get; set; }
    public LocationStatus Status { get; set; }
    public bool IsMainDeparturePoint { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public LocationCoordinateStatus CoordinateStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHarborRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public HarborType Type { get; set; }
    public bool IsMainDeparturePoint { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class UpdateHarborRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public HarborType Type { get; set; }
    public LocationStatus Status { get; set; }
    public bool IsMainDeparturePoint { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
