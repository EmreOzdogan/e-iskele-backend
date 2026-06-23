using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.PaymentNo).HasMaxLength(50);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.GrossTourAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DepositAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.RemainingAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ServiceFeeAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PlatformCommission).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CaptainEarnings).HasColumnType("decimal(18,2)");
        builder.Property(x => x.RefundedAmount).HasColumnType("decimal(18,2)");
        
        builder.HasOne(x => x.Reservation).WithMany().HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Restrict);
    }
}
