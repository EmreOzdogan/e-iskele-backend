using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.Property(x => x.TicketNo).HasMaxLength(50);

        
        builder.HasOne(x => x.User).WithMany(x => x.SupportTickets).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
