using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class BoatFeatureConfiguration : IEntityTypeConfiguration<BoatFeature>
{
    public void Configure(EntityTypeBuilder<BoatFeature> builder)
    {
        builder.ToTable("BoatFeatures");

        builder.HasKey(bf => bf.Id);

        builder.Property(bf => bf.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bf => bf.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bf => bf.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(bf => bf.Boat)
            .WithMany(b => b.BoatFeatures)
            .HasForeignKey(bf => bf.BoatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
