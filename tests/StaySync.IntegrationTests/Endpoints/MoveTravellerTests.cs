using FluentAssertions;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.IntegrationTests.Infra;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace StaySync.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationCollection))]
public class MoveTravellerTests(SqlServerContainerFixture fx)
{
    [Fact]
    public async Task Move_John_Doe_from_0101_to_0102_succeeds_and_updates_snapshots()
    {
        var conn = await fx.CreateUniqueDatabaseAsync();
        var app = new StaySyncApiFactory(conn);
        var client = app.CreateAuthedClient();

        var occ = await client.GetFromJsonAsync<TodayOccupancyDto>("/api/v1/rooms/occupancy/today");
        occ.Should().NotBeNull();
        var assignedOn = occ!.Date;

        var body = new MoveTravellerRequest(
            GroupId: "A12B34",
            Surname: "Doe",
            FirstName: "John",
            DateOfBirth: new DateOnly(1988, 3, 2),
            FromRoomCode: "0101",
            ToRoomCode: "0102",
            AssignedOnDate: assignedOn);

        var resp = await client.PostAsJsonAsync("/api/v1/rooms/move", body);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await resp.Content.ReadFromJsonAsync<MoveTravellerResult>();
        result.Should().NotBeNull();

        result!.FromRoomAfter.RoomCode.Should().Be("0101");
        result.ToRoomAfter.RoomCode.Should().Be("0102");

        result.FromRoomAfter.Travellers.Any(t => t.FirstName.Equals("JOHN", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        result.ToRoomAfter.Travellers.Any(t => t.FirstName.Equals("JOHN", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task Moving_Amir_from_0201_to_full_room_0101_conflicts()
    {
        var conn = await fx.CreateUniqueDatabaseAsync();
        var app = new StaySyncApiFactory(conn);
        var client = app.CreateAuthedClient();

        var occ = await client.GetFromJsonAsync<TodayOccupancyDto>("/api/v1/rooms/occupancy/today");
        occ.Should().NotBeNull();
        var assignedOn = occ!.Date;

        var body = new MoveTravellerRequest(
            GroupId: "A12B34",
            Surname: "Ali",
            FirstName: "Amir",
            DateOfBirth: new DateOnly(1995, 12, 12),
            FromRoomCode: "0201",
            ToRoomCode: "0101",
            AssignedOnDate: assignedOn);

        var resp = await client.PostAsJsonAsync("/api/v1/rooms/move", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
