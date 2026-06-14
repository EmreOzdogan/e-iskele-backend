using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(150);
        
        builder.HasIndex(x => x.Slug).IsUnique();
        
        builder.HasOne(x => x.ParentLocation)
               .WithMany(x => x.ChildLocations)
               .HasForeignKey(x => x.ParentLocationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class HarborConfiguration : IEntityTypeConfiguration<Harbor>
{
    public void Configure(EntityTypeBuilder<Harbor> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        
        builder.HasOne(x => x.Location)
               .WithMany(x => x.Harbors)
               .HasForeignKey(x => x.LocationId)
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.HasIndex(x => x.LocationId);
    }
}
