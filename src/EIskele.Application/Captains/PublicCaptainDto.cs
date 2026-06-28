using System;
using System.Collections.Generic;

namespace EIskele.Application.Captains;

public class PublicCaptainDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int TotalTripsCompleted { get; set; }
    
    public bool IsCompany { get; set; }
    public string? CompanyName { get; set; }
    
    public List<PublicCaptainBoatDto> Boats { get; set; } = new();
}

public class PublicCaptainBoatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal StartingPrice { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    public string LocationName { get; set; } = string.Empty;
}
