using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class CaptainConfiguration : IEntityTypeConfiguration<Captain>
{
    public void Configure(EntityTypeBuilder<Captain> builder)
    {

        builder.HasOne(x => x.User).WithOne(x => x.Captain).HasForeignKey<Captain>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.ApplicationType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.AccountStatus).HasConversion<string>().HasMaxLength(50);
    }
}
