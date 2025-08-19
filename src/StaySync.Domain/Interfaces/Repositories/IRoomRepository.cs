using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByCodeAsync(Guid hotelId, RoomCode code, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid hotelId, RoomCode code, CancellationToken ct = default);
    Task AddAsync(Room room, CancellationToken ct = default);
}
