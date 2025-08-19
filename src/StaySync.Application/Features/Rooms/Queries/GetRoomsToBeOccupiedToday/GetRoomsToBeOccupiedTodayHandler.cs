using StaySync.Application.Interfaces;
using StaySync.Application.Interfaces.Read;

namespace StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;

public sealed class GetRoomsToBeOccupiedTodayHandler(
    ICurrentHotelContext current,
    IHotelLocalClock clock,
    IRoomQueries roomQueries)
    : IQueryHandler<GetRoomsToBeOccupiedTodayQuery, TodayOccupancyDto>
{
    public async Task<TodayOccupancyDto> Handle(GetRoomsToBeOccupiedTodayQuery query, CancellationToken ct)
    {
        var date = query.DateOverride ?? await clock.TodayAsync(current.HotelId, ct);
        return await roomQueries.GetOccupancyForDateAsync(current.HotelId, date, ct);
    }
}
