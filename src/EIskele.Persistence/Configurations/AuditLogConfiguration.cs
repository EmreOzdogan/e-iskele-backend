using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityId).IsRequired().HasMaxLength(50);
        builder.Property(a => a.ActorRole).HasMaxLength(50);
        builder.Property(a => a.IpAddress).HasMaxLength(50);
        builder.Property(a => a.TraceId).HasMaxLength(100);
        builder.Property(a => a.Description).HasMaxLength(500);

        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.EntityType);
        builder.HasIndex(a => a.Action);
    }
}
