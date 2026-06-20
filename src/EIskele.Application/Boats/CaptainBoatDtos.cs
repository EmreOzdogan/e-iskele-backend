using System;
using System.Collections.Generic;

namespace EIskele.Application.Boats;

public class CaptainBoatListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string HarborName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CaptainBoatDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public Guid? HarborId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string ProductionYear { get; set; } = string.Empty;
    public string Length { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}

public class UpdateCaptainBoatRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public Guid? HarborId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string ProductionYear { get; set; } = string.Empty;
    public string Length { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}

public class CreateCaptainBoatRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid LocationId { get; set; }
    public Guid? HarborId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string BoatType { get; set; } = string.Empty;
    public string ProductionYear { get; set; } = string.Empty;
    public string Length { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}
