using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.IntegrationTests.Infra;
using Xunit;

namespace StaySync.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationCollection))]
public class RoomsTodayTests(SqlServerContainerFixture fx)
{
    [Fact]
    public async Task Returns_seeded_occupancy()
    {
        var conn = await fx.CreateUniqueDatabaseAsync();
        var app = new StaySyncApiFactory(conn);
        var client = app.CreateAuthedClient();

        var resp = await client.GetAsync("/api/v1/rooms/occupancy/today");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<TodayOccupancyDto>();
        dto.Should().NotBeNull();
        dto!.Rooms.Should().NotBeEmpty();

        var travellers = dto.Rooms.SelectMany(r => r.Travellers).ToList();

        travellers.Should().Contain(t => t.Surname.Equals("DOE", StringComparison.OrdinalIgnoreCase)
                                      && t.FirstName.Equals("JANE", StringComparison.OrdinalIgnoreCase)
                                      && t.DateOfBirth == new DateOnly(1990, 5, 1));

        travellers.Should().Contain(t => t.Surname.Equals("DOE", StringComparison.OrdinalIgnoreCase)
                                      && t.FirstName.Equals("JOHN", StringComparison.OrdinalIgnoreCase)
                                      && t.DateOfBirth == new DateOnly(1988, 3, 2));

        travellers.Should().Contain(t => t.Surname.Equals("ALI", StringComparison.OrdinalIgnoreCase)
                                      && t.FirstName.Equals("AMIR", StringComparison.OrdinalIgnoreCase)
                                      && t.DateOfBirth == new DateOnly(1995, 12, 12));
    }
}
