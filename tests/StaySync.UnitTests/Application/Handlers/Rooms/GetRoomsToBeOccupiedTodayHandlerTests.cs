using FluentAssertions;
using Moq;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Interfaces.Read;
using StaySync.UnitTests.Application.Handlers.Common;
using Xunit;

namespace StaySync.UnitTests.Application.Handlers.Rooms;

public class GetRoomsToBeOccupiedTodayHandlerTests
{
    [Fact]
    public async Task Returns_today_from_query_service()
    {
        var today = new DateOnly(2025, 8, 14);
        var (hotelId, current, clock) = CommonFakes.HotelAndClock(today);

        var expected = new TodayOccupancyDto(today, new[]
        {
            new RoomOccupancyDto("0101", 2, new[] { new TravellerLiteDto("DOE","JANE", new DateOnly(1990,5,1), "A12B34") })
        });

        var queries = new Mock<IRoomQueries>();
        queries.Setup(q => q.GetOccupancyForDateAsync(hotelId, today, It.IsAny<CancellationToken>()))
               .ReturnsAsync(expected);

        var handler = new GetRoomsToBeOccupiedTodayHandler(current.Object, clock.Object, queries.Object);

        var result = await handler.Handle(new GetRoomsToBeOccupiedTodayQuery(), default);
        result.Should().BeEquivalentTo(expected);
    }
}
