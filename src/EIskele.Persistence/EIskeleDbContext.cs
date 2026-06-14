using EIskele.Domain.Common;
using EIskele.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Persistence;

public class EIskeleDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public EIskeleDbContext(DbContextOptions<EIskeleDbContext> options) : base(options)
    {
    }

    public DbSet<Captain> Captains => Set<Captain>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<BoatFeature> BoatFeatures => Set<BoatFeature>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Harbor> Harbors => Set<Harbor>();
    public DbSet<TourPackage> TourPackages => Set<TourPackage>();
    public DbSet<PackageInclude> PackageIncludes => Set<PackageInclude>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<UserAdminNote> UserAdminNotes => Set<UserAdminNote>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<Payment> Payments => Set<Payment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EIskeleDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        foreach (var entry in ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
