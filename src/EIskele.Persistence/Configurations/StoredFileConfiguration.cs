using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.RelatedEntityType).IsRequired().HasMaxLength(100);
        builder.Property(f => f.RelatedEntityId).IsRequired().HasMaxLength(50);
        builder.Property(f => f.FileType).IsRequired().HasMaxLength(50);
        builder.Property(f => f.OriginalFileName).HasMaxLength(255);
        builder.Property(f => f.StoredFileName).HasMaxLength(255);
        builder.Property(f => f.MimeType).HasMaxLength(100);
        builder.Property(f => f.Extension).HasMaxLength(20);
        builder.Property(f => f.StorageProvider).HasMaxLength(50);
        builder.Property(f => f.StoragePath).HasMaxLength(500);
        builder.Property(f => f.PublicUrl).HasMaxLength(500);
        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(f => f.OwnerUserId);
        builder.HasIndex(f => new { f.RelatedEntityType, f.RelatedEntityId });
    }
}
