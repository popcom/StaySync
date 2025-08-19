using FluentAssertions;
using StaySync.Domain.ValueObjects;
using Xunit;

namespace StaySync.UnitTests.Domain.ValueObjects;

public class TravellerKeyTests
{
    [Fact]
    public void Normalizes_names_to_upper()
    {
        var key = new TravellerKey("Doe", "Jane", new DateOnly(1990, 5, 1));
        key.Surname.Should().Be("DOE");
        key.FirstName.Should().Be("JANE");
        key.DateOfBirth.Should().Be(new DateOnly(1990, 5, 1));
    }
}
