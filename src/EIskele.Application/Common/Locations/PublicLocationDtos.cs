using System;

namespace EIskele.Application.Common.Locations;

public class ActiveLocationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ParentLocationId { get; set; }
}

public class ActiveHarborDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
}
