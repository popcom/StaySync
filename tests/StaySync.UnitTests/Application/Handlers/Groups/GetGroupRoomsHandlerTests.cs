using FluentAssertions;
using Moq;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Exceptions;
using StaySync.UnitTests.Application.Handlers.Common;
using Xunit;

namespace StaySync.UnitTests.Application.Handlers.Groups;

public class GetGroupRoomsHandlerTests
{
    [Fact]
    public async Task Returns_group_rooms()
    {
        var today = new DateOnly(2025, 8, 14);
        var (hotelId, current, clock) = CommonFakes.HotelAndClock(today);

        var expected = new GroupRoomsDto("A12B34", today, [], []);

        var queries = new Mock<IGroupQueries>();
        queries.Setup(q => q.GetGroupRoomsAsync(hotelId, "A12B34", today, It.IsAny<CancellationToken>()))
               .ReturnsAsync(expected);

        var handler = new GetGroupRoomsHandler(current.Object, clock.Object, queries.Object);
        var result = await handler.Handle(new GetGroupRoomsQuery("A12B34"), default);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Not_found_throws()
    {
        var today = new DateOnly(2025, 8, 14);
        var (hotelId, current, clock) = CommonFakes.HotelAndClock(today);

        var queries = new Mock<IGroupQueries>();
        queries.Setup(q => q.GetGroupRoomsAsync(hotelId, "ZZZZZZ", today, It.IsAny<CancellationToken>()))
               .ReturnsAsync((GroupRoomsDto?)null);

        var handler = new GetGroupRoomsHandler(current.Object, clock.Object, queries.Object);

        var act = () => handler.Handle(new GetGroupRoomsQuery("ZZZZZZ"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
