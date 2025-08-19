using StaySync.Application.Interfaces;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Exceptions;

namespace StaySync.Application.Features.Rooms.Queries.GetRoomByCode;

public sealed class GetRoomByCodeHandler(
    ICurrentHotelContext current,
    IHotelLocalClock clock,
    IRoomQueries roomQueries)
    : IQueryHandler<GetRoomByCodeQuery, RoomDetailsDto>
{
    public async Task<RoomDetailsDto> Handle(GetRoomByCodeQuery query, CancellationToken ct)
    {
        var date = query.DateOverride ?? await clock.TodayAsync(current.HotelId, ct);
        var dto = await roomQueries.GetRoomByCodeAsync(current.HotelId, query.RoomCode, date, ct);
        return dto ?? throw new NotFoundException("Room not found for this hotel.");
    }
}
