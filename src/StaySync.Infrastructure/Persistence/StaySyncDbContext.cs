using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;

namespace StaySync.Infrastructure.Persistence;

public sealed class StaySyncDbContext(DbContextOptions<StaySyncDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<TravelGroup> TravelGroups => Set<TravelGroup>();
    public DbSet<Traveller> Travellers => Set<Traveller>();
    public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(StaySyncDbContext).Assembly);
}
