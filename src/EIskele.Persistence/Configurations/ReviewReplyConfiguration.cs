using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class ReviewReplyConfiguration : IEntityTypeConfiguration<ReviewReply>
{
    public void Configure(EntityTypeBuilder<ReviewReply> builder)
    {
        builder.Property(x => x.ReplyText).IsRequired().HasMaxLength(1000);
        
        builder.HasOne(x => x.Review).WithOne(x => x.Reply).HasForeignKey<ReviewReply>(x => x.ReviewId).OnDelete(DeleteBehavior.Cascade);
    }
}
