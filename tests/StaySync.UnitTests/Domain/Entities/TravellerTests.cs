using FluentAssertions;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;
using System;
using Xunit;

namespace StaySync.UnitTests.Domain.Entities;

public class TravellerTests
{
    [Fact]
    public void Creates_traveller_and_normalizes_names()
    {
        var travellerKey = new TravellerKey("Doe", "Jane", new DateOnly(1988, 3, 2));
        var t = new Traveller(Guid.NewGuid(), travellerKey);
        t.Surname.Should().Be("DOE");
        t.FirstName.Should().Be("JANE");
    }

    [Fact]
    public void GroupId_required()
    {
        var act = () => new Traveller(default, new TravellerKey("X", "Y", new DateOnly(2000, 1, 1)));
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null, "Y")]
    [InlineData("X", null)]
    public void Names_required(string? s, string? f)
    {
        var act = () => new Traveller(Guid.NewGuid(), new TravellerKey(s!, f!, new DateOnly(2000, 1, 1)));
        act.Should().Throw<ArgumentException>();
    }
}
