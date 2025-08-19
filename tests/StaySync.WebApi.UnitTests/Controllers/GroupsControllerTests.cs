using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;
using StaySync.Application.Interfaces;
using StaySync.WebApi.Controllers;
using Xunit;

namespace StaySync.WebApi.UnitTests.Controllers;

public class GroupsControllerTests
{
    [Fact]
    public async Task GetRooms_returns_ok()
    {
        var disp = new Mock<IDispatcher>();
        var expected = new GroupRoomsDto("A12B34", new DateOnly(2025, 8, 14), [], []);
        disp.Setup(d => d.Query(It.IsAny<GetGroupRoomsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var ctrl = new GroupsController(disp.Object);
        var res = await ctrl.GetRooms("A12B34", default);

        res.Result.Should().BeOfType<OkObjectResult>();
        (res.Result as OkObjectResult)!.Value.Should().Be(expected);
    }
}
