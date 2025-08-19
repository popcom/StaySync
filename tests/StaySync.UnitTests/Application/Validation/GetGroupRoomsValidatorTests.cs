using FluentAssertions;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;
using Xunit;

namespace StaySync.UnitTests.Application.Validation;

public class GetGroupRoomsValidatorTests
{
    [Fact]
    public void Valid_group_id()
    {
        var v = new GetGroupRoomsValidator();
        var result = v.Validate(new GetGroupRoomsQuery("A12B34"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("012345")]
    [InlineData("1234567")]
    [InlineData("12345")]
    [InlineData("12$456")]
    [InlineData("ABC123")]
    public void Invalid_group_id(string id)
    {
        var v = new GetGroupRoomsValidator();
        var result = v.Validate(new GetGroupRoomsQuery(id));
        result.IsValid.Should().BeFalse();
    }
}
