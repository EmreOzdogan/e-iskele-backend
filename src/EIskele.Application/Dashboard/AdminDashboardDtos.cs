using System;
using System.Collections.Generic;

namespace EIskele.Application.Dashboard;

public class AdminDashboardSummaryResponseDto
{
    public List<DashboardMetricDto> Metrics { get; set; } = new();
    public List<OperationAlertDto> OperationAlerts { get; set; } = new();
    public List<ReservationTrendDto> ReservationTrend { get; set; } = new();
    public List<RecentReservationDto> RecentReservations { get; set; } = new();
    public List<PendingApprovalDto> PendingApprovals { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<PlatformHealthItemDto> PlatformHealth { get; set; } = new();
    public FinanceSummaryDto FinanceSummary { get; set; } = new();
    public QualitySummaryDto QualitySummary { get; set; } = new();
}

public class DashboardMetricDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ChangeLabel { get; set; }
    public string? ChangeType { get; set; } // "positive", "negative", "neutral", "warning"
    public string Icon { get; set; } = string.Empty;
    public string? Href { get; set; }
}

public class OperationAlertDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // "high", "medium", "low", "info"
    public string? Href { get; set; }
}

public class ReservationTrendDto
{
    public string Day { get; set; } = string.Empty;
    public int Reservations { get; set; }
    public int Cancelled { get; set; }
}

public class RecentReservationDto
{
    public string Id { get; set; } = string.Empty;
    public string ReservationNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CaptainName { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PendingApprovalDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "captain", "boat", "package", "document"
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}

public class RecentActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "application", "boat", "reservation", "location", "system"
    public string Description { get; set; } = string.Empty;
    public string CreatedAtText { get; set; } = string.Empty;
    public string? Href { get; set; }
}

public class PlatformHealthItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "active", "warning", "inactive", "planned"
    public string? Description { get; set; }
}

public class FinanceSummaryDto
{
    public decimal MonthlyRevenue { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal PendingPayout { get; set; }
    public int RefundRequests { get; set; }
    public decimal AverageReservationAmount { get; set; }
}

public class QualitySummaryDto
{
    public decimal AverageCaptainRating { get; set; }
    public int NewReviews { get; set; }
    public int Complaints { get; set; }
    public int PendingReviews { get; set; }
    public int LowRatedExperiences { get; set; }
}
