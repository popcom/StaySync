using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Domain.ValueObjects;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.Repositories;

public sealed class TravellerRepository(StaySyncDbContext db) : ITravellerRepository
{
    public Task<Traveller?> GetByIdentityAsync(Guid groupDbId, TravellerKey travellerKey, CancellationToken ct = default)
       => db.Travellers.FirstOrDefaultAsync(
           t => t.GroupId == groupDbId && t.Surname == travellerKey.Surname && t.FirstName == travellerKey.FirstName && t.DateOfBirth == travellerKey.DateOfBirth,
           ct);

    public Task AddAsync(Traveller traveller, CancellationToken ct = default)
        => db.Travellers.AddAsync(traveller, ct).AsTask();
}
