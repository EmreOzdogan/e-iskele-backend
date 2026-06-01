using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class TourPackage : SoftDeletableEntity
{
    public Guid BoatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }
    public ReservationApprovalType ApprovalType { get; set; } = ReservationApprovalType.AutoApprove;

    // Navigation
    public Boat Boat { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
