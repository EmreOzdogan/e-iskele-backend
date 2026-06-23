using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

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
        

        builder.HasOne(x => x.Boat).WithMany(x => x.TourPackages).HasForeignKey(x => x.BoatId).OnDelete(DeleteBehavior.Cascade);
    }
}
