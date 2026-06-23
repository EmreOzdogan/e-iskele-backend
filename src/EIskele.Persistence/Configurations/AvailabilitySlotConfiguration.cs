using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.HasIndex(x => new { x.BoatId, x.StartDateTime, x.EndDateTime });
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Reason).HasMaxLength(500);
        
        builder.HasOne(x => x.Boat).WithMany().HasForeignKey(x => x.BoatId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TourPackage).WithMany().HasForeignKey(x => x.TourPackageId).OnDelete(DeleteBehavior.Restrict);
    }
}
