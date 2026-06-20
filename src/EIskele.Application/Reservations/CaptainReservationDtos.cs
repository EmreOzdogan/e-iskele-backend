using System;

namespace EIskele.Application.Reservations;

public class CaptainReservationListItemDto
{
    public Guid Id { get; set; }
    public string ReservationNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public Guid BoatId { get; set; }
    public string BoatName { get; set; } = string.Empty;
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string Time { get; set; } = string.Empty; // HH:mm - HH:mm
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}

public class CaptainReservationDetailDto : CaptainReservationListItemDto
{
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerNote { get; set; } = string.Empty;
    public string? CaptainNote { get; set; }
    public decimal PricePerPerson { get; set; }
    public string CancellationPolicy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ApproveCaptainReservationRequest
{
    public string? Note { get; set; }
}

public class RejectCaptainReservationRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class PostponeCaptainReservationRequest
{
    public DateTime NewDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class UpdateCaptainReservationNoteRequest
{
    public string Note { get; set; } = string.Empty;
}
