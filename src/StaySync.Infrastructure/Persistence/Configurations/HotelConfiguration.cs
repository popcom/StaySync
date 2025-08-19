using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaySync.Domain.Entities;

namespace StaySync.Infrastructure.Persistence.Configurations;

public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> b)
    {
        b.ToTable("Hotels");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Timezone).IsRequired().HasMaxLength(64);
        b.Property(x => x.ApiKeyHash).IsRequired().HasMaxLength(128);
    }
}
