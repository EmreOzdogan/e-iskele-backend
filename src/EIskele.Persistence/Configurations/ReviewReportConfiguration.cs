using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class ReviewReportConfiguration : IEntityTypeConfiguration<ReviewReport>
{
    public void Configure(EntityTypeBuilder<ReviewReport> builder)
    {
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Message).HasMaxLength(1000);
        builder.Property(x => x.AdminNote).HasMaxLength(1000);
        
        builder.HasOne(x => x.Review).WithMany(x => x.Reports).HasForeignKey(x => x.ReviewId).OnDelete(DeleteBehavior.Cascade);
    }
}
