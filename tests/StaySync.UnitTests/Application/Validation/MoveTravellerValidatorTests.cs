using FluentAssertions;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;
using Xunit;

namespace StaySync.UnitTests.Application.Validation;

public class MoveTravellerValidatorTests
{
    [Fact]
    public void Valid_request()
    {
        var cmd = new MoveTravellerCommand(new(
            GroupId: "A12B34",
            Surname: "Doe",
            FirstName: "John",
            DateOfBirth: new DateOnly(1988, 3, 2),
            FromRoomCode: "0101",
            ToRoomCode: "0102",
            AssignedOnDate: new DateOnly(2025, 8, 14)));

        var v = new MoveTravellerValidator();
        var res = v.Validate(cmd);
        res.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Invalid_request_has_errors()
    {
        var cmd = new MoveTravellerCommand(new(
            GroupId: "012345", // starts with 0
            Surname: "",
            FirstName: "",
            DateOfBirth: new DateOnly(1988, 3, 2),
            FromRoomCode: "1",
            ToRoomCode: "X202",
            AssignedOnDate: default));

        var v = new MoveTravellerValidator();
        var res = v.Validate(cmd);
        res.IsValid.Should().BeFalse();
    }
}
