using StaySync.Domain.Entities;

namespace StaySync.Domain.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Hotel?> GetByApiKeyHashAsync(string apiKeyHash, CancellationToken ct = default); // for API key auth
    Task AddAsync(Hotel hotel, CancellationToken ct = default);
}
