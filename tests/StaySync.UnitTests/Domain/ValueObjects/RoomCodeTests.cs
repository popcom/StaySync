using FluentAssertions;
using StaySync.Domain.ValueObjects;
using StaySync.Domain.Exceptions;
using Xunit;

namespace StaySync.UnitTests.Domain.ValueObjects;

public class RoomCodeTests
{
    [Theory]
    [InlineData("0001")]
    [InlineData("1234")]
    [InlineData("9999")]
    public void Valid_codes_construct(string value)
    {
        var rc = new RoomCode(value);
        rc.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("12345")]
    [InlineData("12a4")]
    public void Invalid_codes_throw(string value)
    {
        var act = () => new RoomCode(value);
        act.Should().Throw<DomainException>();
    }
}
