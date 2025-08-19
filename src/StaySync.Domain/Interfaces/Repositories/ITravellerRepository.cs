using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Interfaces.Repositories;

public interface ITravellerRepository
{
    Task<Traveller?> GetByIdentityAsync(Guid groupDbId, TravellerKey travellerKey, CancellationToken ct = default);
    Task AddAsync(Traveller traveller, CancellationToken ct = default);
}
