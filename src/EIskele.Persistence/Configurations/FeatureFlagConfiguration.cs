using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Key).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.Group).HasMaxLength(100);

        builder.HasIndex(f => f.Key).IsUnique();
    }
}
