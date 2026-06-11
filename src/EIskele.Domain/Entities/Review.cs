using EIskele.Domain.Common;
using EIskele.Domain.Enums;

namespace EIskele.Domain.Entities;

public class Review : BaseEntity, IAuditableEntity, ISoftDeletableEntity
{
    public string ReviewNo { get; set; } = string.Empty;
    
    public Guid CustomerId { get; set; }
    public ApplicationUser Customer { get; set; } = null!;
    
    public Guid BoatId { get; set; }
    public Boat Boat { get; set; } = null!;
    
    public Guid TourPackageId { get; set; }
    public TourPackage TourPackage { get; set; } = null!;
    
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    
    public ReviewStatus Status { get; set; } = ReviewStatus.InReview;
    
    // IAuditableEntity
    public new DateTime CreatedAt { get; set; }
    public new Guid? CreatedBy { get; set; }
    public new DateTime? UpdatedAt { get; set; }
    public new Guid? UpdatedBy { get; set; }
    
    // ISoftDeletableEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
