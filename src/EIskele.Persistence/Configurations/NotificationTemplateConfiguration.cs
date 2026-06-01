using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Channel).IsRequired().HasMaxLength(50);
        builder.Property(t => t.SubjectTemplate).IsRequired().HasMaxLength(255);
        builder.Property(t => t.BodyTemplate).IsRequired();

        builder.HasIndex(t => new { t.Code, t.Channel }).IsUnique();
    }
}
