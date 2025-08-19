using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.IntegrationTests.Infra;
using Xunit;

namespace StaySync.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationCollection))]
public class RoomByCodeTests(SqlServerContainerFixture fx)
{
    [Fact]
    public async Task Returns_room_0101_with_two_travellers_from_seed()
    {
        var conn = await fx.CreateUniqueDatabaseAsync();
        var app = new StaySyncApiFactory(conn); ;
        var client = app.CreateAuthedClient();

        var resp = await client.GetAsync("/api/v1/rooms/0101");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<RoomDetailsDto>();
        dto.Should().NotBeNull();
        dto!.RoomCode.Should().Be("0101");
        dto.Travellers.Should().HaveCount(2);
        dto.Travellers.Select(t => t.Surname).Should().AllBeEquivalentTo("DOE");
    }
}
