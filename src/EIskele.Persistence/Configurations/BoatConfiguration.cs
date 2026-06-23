using EIskele.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EIskele.Persistence.Configurations;

public class BoatConfiguration : IEntityTypeConfiguration<Boat>
{
    public void Configure(EntityTypeBuilder<Boat> builder)
    {

        builder.HasOne(x => x.Captain).WithMany(x => x.Boats).HasForeignKey(x => x.CaptainId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany(x => x.Boats).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Harbor).WithMany(x => x.Boats).HasForeignKey(x => x.HarborId).OnDelete(DeleteBehavior.Restrict);
    }
}
