using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class UserLegalAgreementConfiguration : IEntityTypeConfiguration<UserLegalAgreement>
{
    public void Configure(EntityTypeBuilder<UserLegalAgreement> builder)
    {
        builder.Property(x => x.AgreementName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Version).HasMaxLength(20);
        builder.Property(x => x.Status).HasMaxLength(20);
        
        builder.HasOne(x => x.User).WithMany(x => x.LegalAgreements).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
