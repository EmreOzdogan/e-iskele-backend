using Microsoft.AspNetCore.Identity;
using EIskele.Domain.Common;

namespace EIskele.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, IAuditableEntity
{
    public string Description { get; set; } = string.Empty;
    
    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
