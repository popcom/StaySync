using FluentAssertions;
using Moq;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Exceptions;
using StaySync.UnitTests.Application.Handlers.Common;
using Xunit;

namespace StaySync.UnitTests.Application.Handlers.Rooms;

public class GetRoomByCodeHandlerTests
{
    [Fact]
    public async Task Returns_room_details()
    {
        var today = new DateOnly(2025, 8, 14);
        var (hotelId, current, clock) = CommonFakes.HotelAndClock(today);

        var dto = new RoomDetailsDto("0101", 2, new[]
        {
            new RoomDetailsTravellerDto("DOE","JANE", new DateOnly(1990,5,1), "A12B34")
        }, today);

        var queries = new Mock<IRoomQueries>();
        queries.Setup(q => q.GetRoomByCodeAsync(hotelId, "0101", today, It.IsAny<CancellationToken>()))
               .ReturnsAsync(dto);

        var handler = new GetRoomByCodeHandler(current.Object, clock.Object, queries.Object);

        var result = await handler.Handle(new GetRoomByCodeQuery("0101"), default);
        result.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task Not_found_throws()
    {
        var today = new DateOnly(2025, 8, 14);
        var (hotelId, current, clock) = CommonFakes.HotelAndClock(today);

        var queries = new Mock<IRoomQueries>();
        queries.Setup(q => q.GetRoomByCodeAsync(hotelId, "9999", today, It.IsAny<CancellationToken>()))
               .ReturnsAsync((RoomDetailsDto?)null);

        var handler = new GetRoomByCodeHandler(current.Object, clock.Object, queries.Object);

        var act = () => handler.Handle(new GetRoomByCodeQuery("9999"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
