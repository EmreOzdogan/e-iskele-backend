using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.Property(p => p.Group).HasMaxLength(100);

        builder.HasIndex(p => p.Code).IsUnique();
    }
}
