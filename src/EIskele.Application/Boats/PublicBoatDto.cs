using System;
using System.Collections.Generic;

namespace EIskele.Application.Boats;

public class PublicBoatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Summary Fields
    public int Capacity { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public decimal StartingPrice { get; set; }
    
    // Relations
    public Guid CaptainId { get; set; }
    public PublicCaptainSummaryDto Captain { get; set; } = null!;
    
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    
    public Guid HarborId { get; set; }
    public string HarborName { get; set; } = string.Empty;
    
    // Media & Features
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Features { get; set; } = new();
}

public class PublicCaptainSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
