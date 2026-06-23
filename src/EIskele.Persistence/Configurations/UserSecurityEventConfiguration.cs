using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class UserSecurityEventConfiguration : IEntityTypeConfiguration<UserSecurityEvent>
{
    public void Configure(EntityTypeBuilder<UserSecurityEvent> builder)
    {
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.IpAddress).HasMaxLength(50);
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        
        builder.HasOne(x => x.User).WithMany(x => x.SecurityEvents).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
