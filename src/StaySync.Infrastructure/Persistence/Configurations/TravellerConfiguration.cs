using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaySync.Domain.Entities;

namespace StaySync.Infrastructure.Persistence.Configurations;

public sealed class TravellerConfiguration : IEntityTypeConfiguration<Traveller>
{
    public void Configure(EntityTypeBuilder<Traveller> b)
    {
        b.ToTable("Travellers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Surname).IsRequired().HasMaxLength(100);
        b.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        b.Property(x => x.DateOfBirth).HasColumnType("date").IsRequired();

        b.HasOne<TravelGroup>()
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique traveller identity within a group (Surname/FirstName are stored uppercase in Domain)
        b.HasIndex(x => new { x.GroupId, x.Surname, x.FirstName, x.DateOfBirth }).IsUnique();
    }
}
