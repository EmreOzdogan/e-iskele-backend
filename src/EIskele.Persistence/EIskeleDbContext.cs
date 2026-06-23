using EIskele.Application.Common.Security;
using EIskele.Domain.Common;
using EIskele.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EIskele.Persistence.Extensions;

namespace EIskele.Persistence;

public class EIskeleDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICurrentUserService? _currentUserService;

    public EIskeleDbContext(DbContextOptions<EIskeleDbContext> options, ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
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
    public DbSet<ReviewReply> ReviewReplies => Set<ReviewReply>();
    public DbSet<ReviewReport> ReviewReports => Set<ReviewReport>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<UserNotificationPreference> UserNotificationPreferences => Set<UserNotificationPreference>();
    public DbSet<UserLegalAgreement> UserLegalAgreements => Set<UserLegalAgreement>();
    public DbSet<UserSecurityEvent> UserSecurityEvents => Set<UserSecurityEvent>();
    public DbSet<UserActiveSession> UserActiveSessions => Set<UserActiveSession>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EIskeleDbContext).Assembly);
        modelBuilder.ApplySoftDeleteQueryFilter();
    }

    public override int SaveChanges()
    {
        ApplyAuditLogic();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditLogic();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditLogic()
    {
        var userId = _currentUserService?.UserId;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
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
                    entry.Entity.DeletedBy = userId;
                    break;
            }
        }
    }
}
