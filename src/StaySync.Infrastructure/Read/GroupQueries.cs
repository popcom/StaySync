using Dapper;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;
using StaySync.Application.Interfaces.Read;
using System.Linq;

namespace StaySync.Infrastructure.Read;

internal sealed class GroupQueries(ISqlConnectionFactory factory) : IGroupQueries
{
    public async Task<GroupRoomsDto?> GetGroupRoomsAsync(Guid hotelId, string groupId, DateOnly? date, CancellationToken ct)
    {
        const string sqlGroup = """
            SELECT TOP 1 g.Id
            FROM TravelGroups g
            WHERE g.HotelId = @HotelId AND g.GroupId = @GroupId;
        """;

        using var conn = factory.Create();

        var groupDbId = await conn.ExecuteScalarAsync<Guid?>(
            new CommandDefinition(sqlGroup, new { HotelId = hotelId, GroupId = groupId }, cancellationToken: ct));

        if (groupDbId is null) return null;

        if (date is null)
        {
            // No date → return empty occupancy sets, only metadata
            return new GroupRoomsDto(groupId, null, [], []);
        }

        const string sqlRooms = """
            SELECT r.RoomCode, r.BedCount, t.Surname, t.FirstName, t.DateOfBirth
            FROM RoomAssignments a
            JOIN Rooms r ON r.Id = a.RoomId
            JOIN Travellers t ON t.Id = a.TravellerId
            WHERE a.HotelId = @HotelId AND a.AssignedOnDate = @Date AND t.GroupId = @GroupDbId
            ORDER BY r.RoomCode, t.Surname, t.FirstName;
        """;

        const string sqlUnassigned = """
            SELECT t.Surname, t.FirstName, t.DateOfBirth
            FROM Travellers t
            WHERE t.GroupId = @GroupDbId
              AND NOT EXISTS (
                  SELECT 1 FROM RoomAssignments a
                  WHERE a.TravellerId = t.Id AND a.AssignedOnDate = @Date
              )
            ORDER BY t.Surname, t.FirstName;
        """;

        var rows = await conn.QueryAsync<RoomRow>(new(sqlRooms, new { HotelId = hotelId, Date = date, GroupDbId = groupDbId }, cancellationToken: ct));
        var unassigned = await conn.QueryAsync<TravRow>(new(sqlUnassigned, new { GroupDbId = groupDbId, Date = date }, cancellationToken: ct));

        var roomDtos = rows.GroupBy(x => new { x.RoomCode, x.BedCount })
            .Select(g => new GroupRoomOccupancyDto(
                g.Key.RoomCode, g.Key.BedCount,
                g.Select(t => new GroupTravellerDto(t.Surname, t.FirstName, t.DateOfBirth)).ToList()))
            .OrderBy(r => r.RoomCode)
            .ToList();

        var unassignedDtos = unassigned.Select(t => new GroupTravellerDto(t.Surname, t.FirstName, t.DateOfBirth)).ToList();

        return new GroupRoomsDto(groupId, date, roomDtos, unassignedDtos);       
    }

    // local record for mapping
    record RoomRow(string RoomCode, int BedCount, string Surname, string FirstName, DateOnly DateOfBirth);
    // local record for mapping
    record TravRow(string Surname, string FirstName, DateOnly DateOfBirth);
}
