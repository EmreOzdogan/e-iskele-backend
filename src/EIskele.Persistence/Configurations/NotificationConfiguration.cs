using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Channel).HasConversion<string>().IsRequired().HasMaxLength(50);
        builder.Property(n => n.Type).HasConversion<string>().IsRequired().HasMaxLength(100);
        builder.Property(n => n.Subject).IsRequired().HasMaxLength(255);
        builder.Property(n => n.Status).HasConversion<string>().IsRequired().HasMaxLength(50);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.Status);
    }
}
