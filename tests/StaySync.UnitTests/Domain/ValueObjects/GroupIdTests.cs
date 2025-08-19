using FluentAssertions;
using StaySync.Domain.ValueObjects;
using StaySync.Domain.Exceptions;
using Xunit;

namespace StaySync.UnitTests.Domain.ValueObjects;

public class GroupIdTests
{
    [Theory]
    [InlineData("A12B34")]
    [InlineData("12AB34")]
    [InlineData("1A2B34")]
    [InlineData("1234AB")]
    public void Valid_ids_construct_uppercased(string id)
    {
        var gid = new GroupId(id);
        gid.Value.Should().Be(id.ToUpperInvariant());
    }

    [Theory]
    [InlineData("012345")]      // starts with 0
    [InlineData("1234567")]     // too long
    [InlineData("12345")]       // too short
    [InlineData("12$456")]      // non-alnum
    [InlineData("ABC123")]      // >2 letters
    public void Invalid_ids_throw(string id)
    {
        var act = () => new GroupId(id);
        act.Should().Throw<DomainException>();
    }
}
