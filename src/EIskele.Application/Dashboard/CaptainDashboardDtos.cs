using System;
using System.Collections.Generic;

namespace EIskele.Application.Dashboard;

public class CaptainDashboardDataDto
{
    public string CaptainName { get; set; } = string.Empty;
    public string TodayText { get; set; } = string.Empty;
    public string OperationStatus { get; set; } = "active"; // "active", "limited", "suspended"
    public int ProfileCompletionRate { get; set; }
    public List<CaptainDashboardMetricDto> Metrics { get; set; } = new();
    public List<CaptainDashboardReservationDto> TodayReservations { get; set; } = new();
    public List<CaptainDashboardReservationDto> PendingReservations { get; set; } = new();
    public List<CaptainDashboardBoatStatusDto> BoatStatuses { get; set; } = new();
    public List<CaptainDashboardReviewDto> RecentReviews { get; set; } = new();
    public CaptainDashboardCalendarSummaryDto CalendarSummary { get; set; } = new();
    public CaptainDashboardEarningsSummaryDto EarningsSummary { get; set; } = new();
}

public class CaptainDashboardCalendarSummaryDto
{
    public int OccupancyRate { get; set; }
    public int AvailableDays { get; set; }
    public int BookedDays { get; set; }
}

public class CaptainDashboardEarningsSummaryDto
{
    public decimal TotalEarnings { get; set; }
    public decimal CompletedPayments { get; set; }
    public decimal PendingPayments { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal EstimatedCaptainEarnings { get; set; }
}

public class CaptainDashboardMetricDto
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public object Value { get; set; } = string.Empty; // numeric or string (e.g. 2 or "₺42.500")
    public string? Description { get; set; }
    public string? TrendText { get; set; }
    public string? Status { get; set; } // "default" | "success" | "warning" | "danger" | "info"
}

public class CaptainDashboardReservationDto
{
    public string Id { get; set; } = string.Empty;
    public string ReservationNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string TourTitle { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // ReservationStatus
    public string PaymentStatus { get; set; } = string.Empty; // PaymentStatus
}

public class CaptainDashboardBoatStatusDto
{
    public string Id { get; set; } = string.Empty;
    public string BoatName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // BoatStatus
    public int ActivePackageCount { get; set; }
    public string CalendarStatus { get; set; } = string.Empty; // CalendarStatus
    public string LastUpdatedText { get; set; } = string.Empty;
}

public class CaptainDashboardReviewDto
{
    public string Id { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
}
