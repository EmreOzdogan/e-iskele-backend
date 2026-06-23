using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.Property(x => x.PayoutNo).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.IbanMasked).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(500);
        
        builder.HasIndex(x => x.PayoutNo).IsUnique();
        builder.HasIndex(x => new { x.CaptainId, x.Status });
        
        builder.HasOne(x => x.Captain).WithMany().HasForeignKey(x => x.CaptainId).OnDelete(DeleteBehavior.Restrict);
    }
}
