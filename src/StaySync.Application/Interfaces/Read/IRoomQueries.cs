using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;

namespace StaySync.Application.Interfaces.Read;

public interface IRoomQueries
{
    Task<TodayOccupancyDto> GetOccupancyForDateAsync(Guid hotelId, DateOnly date, CancellationToken ct);
    Task<RoomDetailsDto?> GetRoomByCodeAsync(Guid hotelId, string roomCode, DateOnly date, CancellationToken ct);
}
