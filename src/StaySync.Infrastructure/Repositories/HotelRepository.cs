using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.Repositories;

public sealed class HotelRepository(StaySyncDbContext db) : IHotelRepository
{
    public Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Hotels.FirstOrDefaultAsync(h => h.Id == id, ct);

    public Task<Hotel?> GetByApiKeyHashAsync(string apiKeyHash, CancellationToken ct = default)
        => db.Hotels.FirstOrDefaultAsync(h => h.ApiKeyHash == apiKeyHash, ct);

    public Task AddAsync(Hotel hotel, CancellationToken ct = default)
        => db.Hotels.AddAsync(hotel, ct).AsTask();
}
