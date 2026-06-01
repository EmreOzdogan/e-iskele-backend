using Microsoft.AspNetCore.Identity;
using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, ISoftDeletableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // ISoftDeletableEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public Captain? Captain { get; set; }
}
