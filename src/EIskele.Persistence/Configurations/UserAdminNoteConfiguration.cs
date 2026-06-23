using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class UserAdminNoteConfiguration : IEntityTypeConfiguration<UserAdminNote>
{
    public void Configure(EntityTypeBuilder<UserAdminNote> builder)
    {
        builder.HasOne(x => x.User).WithMany(x => x.AdminNotes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
