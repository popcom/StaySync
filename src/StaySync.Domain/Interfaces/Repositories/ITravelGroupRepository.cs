using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Interfaces.Repositories;

public interface ITravelGroupRepository
{
    Task<TravelGroup?> GetByGroupIdAsync(Guid hotelId, GroupId groupId, CancellationToken ct = default);
    Task AddAsync(TravelGroup group, CancellationToken ct = default);
}
