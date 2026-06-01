using System;

namespace EIskele.Application.Boats;

public class CreateBoatRequest
{
    public Guid CaptainId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateTourPackageRequest
{
    public Guid BoatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
}

public class BoatResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TourPackageResponse
{
    public Guid Id { get; set; }
}
