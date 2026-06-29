namespace EIskele.Application.Packages;

public class PackageStatsDto
{
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InReviewCount { get; set; }
    public int PassiveCount { get; set; }
    public int RejectedCount { get; set; }
    public int RevisionRequestedCount { get; set; }
    public int FeaturedCount { get; set; }
    public decimal AveragePrice { get; set; }
}
