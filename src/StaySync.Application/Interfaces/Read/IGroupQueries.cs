using StaySync.Application.Features.Groups.Queries.GetGroupRooms;

namespace StaySync.Application.Interfaces.Read;

public interface IGroupQueries
{
    Task<GroupRoomsDto?> GetGroupRoomsAsync(Guid hotelId, string groupId, DateOnly? date, CancellationToken ct);
}
