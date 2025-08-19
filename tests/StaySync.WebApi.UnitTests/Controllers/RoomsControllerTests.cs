using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Interfaces;
using StaySync.WebApi.Controllers;
using Xunit;

namespace StaySync.WebApi.UnitTests.Controllers;

public class RoomsControllerTests
{
    [Fact]
    public async Task GetToday_returns_ok()
    {
        var disp = new Mock<IDispatcher>();
        var expected = new TodayOccupancyDto(new DateOnly(2025, 8, 14), []);
        disp.Setup(d => d.Query(It.IsAny<GetRoomsToBeOccupiedTodayQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var ctrl = new RoomsController(disp.Object);
        var res = await ctrl.GetToday(default);

        res.Result.Should().BeOfType<OkObjectResult>();
        (res.Result as OkObjectResult)!.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetByCode_returns_ok()
    {
        var disp = new Mock<IDispatcher>();
        var expected = new RoomDetailsDto("0101", 2, [], new DateOnly(2025, 8, 14));
        disp.Setup(d => d.Query(It.IsAny<GetRoomByCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var ctrl = new RoomsController(disp.Object);
        var res = await ctrl.GetByCode("0101", default);

        res.Result.Should().BeOfType<OkObjectResult>();
        (res.Result as OkObjectResult)!.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Move_returns_ok()
    {
        var disp = new Mock<IDispatcher>();
        var expected = new MoveTravellerResult(new DateOnly(2025, 8, 14),
            new("0101", 2, []), new("0102", 2, []));
        disp.Setup(d => d.Send(It.IsAny<MoveTravellerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var ctrl = new RoomsController(disp.Object);
        var res = await ctrl.Move(new("A12B34", "Doe", "John", new DateOnly(1988, 3, 2), "0101", "0102", new DateOnly(2025, 8, 14)), default);

        res.Result.Should().BeOfType<OkObjectResult>();
        (res.Result as OkObjectResult)!.Value.Should().Be(expected);
    }
}
