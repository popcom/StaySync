using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;

namespace StaySync.Infrastructure.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> b)
    {
        b.ToTable("Rooms");
        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.BedCount).IsRequired();

        b.Property(x => x.RoomCode)
            .HasConversion(v => v.Value, v => new RoomCode(v))
            .HasColumnType("char(4)")
            .IsRequired();

        b.HasIndex(x => new { x.HotelId, x.RoomCode }).IsUnique();

        b.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
