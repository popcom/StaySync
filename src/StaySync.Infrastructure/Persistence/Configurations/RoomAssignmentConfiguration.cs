using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaySync.Domain.Entities;

namespace StaySync.Infrastructure.Persistence.Configurations;

public sealed class RoomAssignmentConfiguration : IEntityTypeConfiguration<RoomAssignment>
{
    public void Configure(EntityTypeBuilder<RoomAssignment> b)
    {
        b.ToTable("RoomAssignments");
        b.HasKey(x => x.Id);

        b.Property(x => x.AssignedOnDate).HasColumnType("date").IsRequired();

        // Shadow concurrency token
        b.Property<byte[]>("RowVersion").IsRowVersion();

        b.HasOne<Hotel>().WithMany().HasForeignKey(x => x.HotelId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Traveller>().WithMany().HasForeignKey(x => x.TravellerId).OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate assignment of same traveller to same room/date
        b.HasIndex(x => new { x.RoomId, x.AssignedOnDate, x.TravellerId }).IsUnique();

        // Helpful read index for “today” queries
        b.HasIndex(x => new { x.AssignedOnDate, x.HotelId });
    }
}
