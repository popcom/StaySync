using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Domain.ValueObjects;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.Repositories;

public sealed class RoomRepository(StaySyncDbContext db) : IRoomRepository
{
    public Task<Room?> GetByCodeAsync(Guid hotelId, RoomCode code, CancellationToken ct = default)
        => db.Rooms.FirstOrDefaultAsync(r => r.HotelId == hotelId && r.RoomCode == code, ct);

    public Task<bool> ExistsAsync(Guid hotelId, RoomCode code, CancellationToken ct = default)
        => db.Rooms.AnyAsync(r => r.HotelId == hotelId && r.RoomCode == code, ct);

    public Task AddAsync(Room room, CancellationToken ct = default)
        => db.Rooms.AddAsync(room, ct).AsTask();
}
