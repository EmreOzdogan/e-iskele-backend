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
        builder.Property(x => x.UserNo).HasMaxLength(50);
        
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
        builder.HasOne(x => x.Harbor).WithMany(x => x.Boats).HasForeignKey(x => x.HarborId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TourPackageConfiguration : IEntityTypeConfiguration<TourPackage>
{
    public void Configure(EntityTypeBuilder<TourPackage> builder)
    {
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PrepaymentPercentage).HasColumnType("decimal(5,2)");
        builder.Property(x => x.ServiceFee).HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.TourType).HasMaxLength(100);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.Currency).HasMaxLength(10);
        builder.Property(x => x.CancellationPolicyType).HasMaxLength(100);
        
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

public class UserAdminNoteConfiguration : IEntityTypeConfiguration<UserAdminNote>
{
    public void Configure(EntityTypeBuilder<UserAdminNote> builder)
    {
        builder.HasOne(x => x.User).WithMany(x => x.AdminNotes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.ReviewNo).HasMaxLength(50);
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.HasOne(x => x.Customer).WithMany(x => x.Reviews).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Boat).WithMany().HasForeignKey(x => x.BoatId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TourPackage).WithMany().HasForeignKey(x => x.TourPackageId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.Property(x => x.TicketNo).HasMaxLength(50);
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.HasOne(x => x.User).WithMany(x => x.SupportTickets).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.PaymentNo).HasMaxLength(50);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PlatformCommission).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CaptainEarnings).HasColumnType("decimal(18,2)");
        
        builder.HasOne(x => x.Reservation).WithMany().HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Restrict);
    }
}
