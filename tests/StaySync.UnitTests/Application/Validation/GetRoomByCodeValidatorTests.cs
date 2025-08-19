using FluentAssertions;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using Xunit;

namespace StaySync.UnitTests.Application.Validation;

public class GetRoomByCodeValidatorTests
{
    [Fact]
    public void Valid_room_code()
    {
        var v = new GetRoomByCodeValidator();
        var result = v.Validate(new GetRoomByCodeQuery("0123"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("12345")]
    [InlineData("12a4")]
    public void Invalid_room_code(string code)
    {
        var v = new GetRoomByCodeValidator();
        var result = v.Validate(new GetRoomByCodeQuery(code));
        result.IsValid.Should().BeFalse();
    }
}
