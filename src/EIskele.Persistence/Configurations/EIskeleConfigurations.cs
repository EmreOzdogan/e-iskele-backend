using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CaptainConfiguration : IEntityTypeConfiguration<Captain>
{
    public void Configure(EntityTypeBuilder<Captain> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasOne(x => x.User).WithOne(x => x.Captain).HasForeignKey<Captain>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class BoatConfiguration : IEntityTypeConfiguration<Boat>
{
    public void Configure(EntityTypeBuilder<Boat> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasOne(x => x.Captain).WithMany(x => x.Boats).HasForeignKey(x => x.CaptainId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany(x => x.Boats).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TourPackageConfiguration : IEntityTypeConfiguration<TourPackage>
{
    public void Configure(EntityTypeBuilder<TourPackage> builder)
    {
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasOne(x => x.Boat).WithMany(x => x.TourPackages).HasForeignKey(x => x.BoatId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
        
        builder.HasOne(x => x.Customer).WithMany(x => x.Reservations).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Boat).WithMany(x => x.Reservations).HasForeignKey(x => x.BoatId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TourPackage).WithMany(x => x.Reservations).HasForeignKey(x => x.TourPackageId).OnDelete(DeleteBehavior.Restrict);
    }
}
