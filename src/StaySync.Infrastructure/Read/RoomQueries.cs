using System.Data;
using Dapper;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Interfaces.Read;
using System.Linq;

namespace StaySync.Infrastructure.Read;

internal sealed class RoomQueries(ISqlConnectionFactory factory) : IRoomQueries
{
    public async Task<TodayOccupancyDto> GetOccupancyForDateAsync(Guid hotelId, DateOnly date, CancellationToken ct)
    {
        const string sql = """
            SELECT r.RoomCode, r.BedCount,
                   t.Surname, t.FirstName, t.DateOfBirth, g.GroupId
            FROM RoomAssignments a
            JOIN Rooms r ON r.Id = a.RoomId
            JOIN Travellers t ON t.Id = a.TravellerId
            JOIN TravelGroups g ON g.Id = t.GroupId
            WHERE a.HotelId = @HotelId AND a.AssignedOnDate = @Date
            ORDER BY r.RoomCode, t.Surname, t.FirstName
        """;

        using var conn = factory.Create();
        var rows = await conn.QueryAsync<Row>(new CommandDefinition(
            sql, new { HotelId = hotelId, Date = date }, cancellationToken: ct));

        var grouped = rows.GroupBy(x => new { x.RoomCode, x.BedCount })
            .Select(g => new RoomOccupancyDto(
                g.Key.RoomCode, g.Key.BedCount,
                g.Select(t => new TravellerLiteDto(t.Surname, t.FirstName, t.DateOfBirth, t.GroupId)).ToList()))
            .OrderBy(r => r.RoomCode)
            .ToList();

        return new TodayOccupancyDto(date, grouped);

    }

    // local record for mapping
    record Row(string RoomCode, int BedCount, string Surname, string FirstName, DateOnly DateOfBirth, string GroupId);

    public async Task<RoomDetailsDto?> GetRoomByCodeAsync(Guid hotelId, string roomCode, DateOnly date, CancellationToken ct)
    {
        const string sql = """
            SELECT TOP 1 r.BedCount
            FROM Rooms r
            WHERE r.HotelId = @HotelId AND r.RoomCode = @RoomCode;

            SELECT t.Surname, t.FirstName, t.DateOfBirth, g.GroupId
            FROM RoomAssignments a
            JOIN Rooms r ON r.Id = a.RoomId
            JOIN Travellers t ON t.Id = a.TravellerId
            JOIN TravelGroups g ON g.Id = t.GroupId
            WHERE a.HotelId = @HotelId AND a.AssignedOnDate = @Date AND r.RoomCode = @RoomCode
            ORDER BY t.Surname, t.FirstName;
        """;

        using var conn = factory.Create();
        using var multi = await conn.QueryMultipleAsync(new CommandDefinition(
            sql, new { HotelId = hotelId, RoomCode = roomCode, Date = date }, cancellationToken: ct));

        var bedCount = await multi.ReadFirstOrDefaultAsync<int?>();
        if (bedCount is null) return null;

        var travellers = (await multi.ReadAsync<TravellerRow>()).Select(t =>
            new RoomDetailsTravellerDto(t.Surname, t.FirstName, t.DateOfBirth, t.GroupId)).ToList();

        return new RoomDetailsDto(roomCode, bedCount.Value, travellers, date);
    }

    // local record for mapping
    record TravellerRow(string Surname, string FirstName, DateOnly DateOfBirth, string GroupId);
}
