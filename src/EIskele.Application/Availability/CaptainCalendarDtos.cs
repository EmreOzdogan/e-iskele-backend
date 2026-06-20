using System;
using System.ComponentModel.DataAnnotations;

namespace EIskele.Application.Availability;

public class CaptainCalendarSlotDto
{
    public Guid Id { get; set; }
    public Guid BoatId { get; set; }
    public Guid? PackageId { get; set; }
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string EndTime { get; set; } = string.Empty; // HH:mm
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int RemainingCapacity { get; set; }
    public decimal? Price { get; set; }
    public string? Note { get; set; }
    
    // İlişkisel detaylar (UI için)
    public string? BoatName { get; set; }
    public string? PackageName { get; set; }
    public int? ReservationsCount { get; set; }
}

public class CaptainCalendarMetricsDto
{
    public int AvailableDaysThisMonth { get; set; }
    public int BookedDaysThisMonth { get; set; }
    public int ClosedDays { get; set; }
    public int MaintenanceDays { get; set; }
    public int PendingReservations { get; set; }
    public string LastUpdatedAt { get; set; } = string.Empty; // ISO date
}

public class CaptainCalendarFilterDto
{
    public Guid? BoatId { get; set; }
    public Guid? PackageId { get; set; }
    public string Status { get; set; } = "all";
    public string View { get; set; } = "month";
    public DateTime Date { get; set; }
}

public class AddAvailabilityBlockRequest
{
    [Required]
    public Guid BoatId { get; set; }
    public Guid? PackageId { get; set; }
    
    [Required]
    public string StartTime { get; set; } = string.Empty; // HH:mm
    
    [Required]
    public string EndTime { get; set; } = string.Empty; // HH:mm
    
    public int? Capacity { get; set; }
    public string Recurrence { get; set; } = "none";
    public string? Note { get; set; }
    
    [Required]
    public DateTime SelectedDate { get; set; }
}
