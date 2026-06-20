using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Reservation : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid BoatId { get; set; }
    public Guid TourPackageId { get; set; }
    
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int GuestCount { get; set; }
    
    // Snapshot of price at the time of booking
    public string ReservationNo { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string PackageNameSnapshot { get; set; } = string.Empty;
    
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? CaptainNote { get; set; }

    // Navigation
    public ApplicationUser Customer { get; set; } = null!;
    public Boat Boat { get; set; } = null!;
    public TourPackage TourPackage { get; set; } = null!;
}
