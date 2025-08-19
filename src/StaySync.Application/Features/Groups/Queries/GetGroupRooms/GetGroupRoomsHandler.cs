using StaySync.Application.Interfaces;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Exceptions;

namespace StaySync.Application.Features.Groups.Queries.GetGroupRooms;

public sealed class GetGroupRoomsHandler(
    ICurrentHotelContext current,
    IHotelLocalClock clock,
    IGroupQueries groupQueries)
    : IQueryHandler<GetGroupRoomsQuery, GroupRoomsDto>
{
    public async Task<GroupRoomsDto> Handle(GetGroupRoomsQuery query, CancellationToken ct)
    {
        var date = query.DateOverride ?? await clock.TodayAsync(current.HotelId, ct);
        var dto = await groupQueries.GetGroupRoomsAsync(current.HotelId, query.GroupId, date, ct);
        return dto ?? throw new NotFoundException("Group not found for this hotel.");
    }
}
