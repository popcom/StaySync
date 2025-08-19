using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Domain.ValueObjects;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.Repositories;

public sealed class TravelGroupRepository(StaySyncDbContext db) : ITravelGroupRepository
{
    public Task<TravelGroup?> GetByGroupIdAsync(Guid hotelId, GroupId groupId, CancellationToken ct = default)
        => db.TravelGroups.FirstOrDefaultAsync(g => g.HotelId == hotelId && g.GroupId == groupId, ct);

    public Task AddAsync(TravelGroup group, CancellationToken ct = default)
        => db.TravelGroups.AddAsync(group, ct).AsTask();
}
