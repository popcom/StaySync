using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;

namespace StaySync.Infrastructure.Persistence.Configurations;

public sealed class TravelGroupConfiguration : IEntityTypeConfiguration<TravelGroup>
{
    public void Configure(EntityTypeBuilder<TravelGroup> b)
    {
        b.ToTable("TravelGroups");
        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.ArrivalDate).HasColumnType("date").IsRequired();
        b.Property(x => x.TravellerCount).IsRequired();

        b.Property(x => x.GroupId)
            .HasConversion(v => v.Value, v => new GroupId(v))
            .HasColumnType("char(6)")
            .IsRequired();

        b.HasIndex(x => new { x.HotelId, x.GroupId }).IsUnique();

        b.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
