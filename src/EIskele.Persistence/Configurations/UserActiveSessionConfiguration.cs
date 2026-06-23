using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class UserActiveSessionConfiguration : IEntityTypeConfiguration<UserActiveSession>
{
    public void Configure(EntityTypeBuilder<UserActiveSession> builder)
    {
        builder.Property(x => x.Device).HasMaxLength(200);
        builder.Property(x => x.Location).HasMaxLength(200);
        builder.Property(x => x.IpAddress).HasMaxLength(50);
        
        builder.HasOne(x => x.User).WithMany(x => x.ActiveSessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
