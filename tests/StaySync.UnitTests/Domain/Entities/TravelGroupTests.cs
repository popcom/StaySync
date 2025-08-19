using System;
using FluentAssertions;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;
using Xunit;

namespace StaySync.UnitTests.Domain.Entities;

public class TravelGroupTests
{
    [Fact]
    public void Creates_group()
    {
        var g = new TravelGroup(Guid.NewGuid(), new GroupId("A12B34"), new DateOnly(2025, 8, 1), 3);
        g.TravellerCount.Should().Be(3);
    }

    [Fact]
    public void TravellerCount_must_be_positive()
    {
        var act = () => new TravelGroup(Guid.NewGuid(), new GroupId("A12B34"), new DateOnly(2025, 8, 1), 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void HotelId_required()
    {
        var act = () => new TravelGroup(default, new GroupId("A12B34"), new DateOnly(2025, 8, 1), 2);
        act.Should().Throw<ArgumentException>();
    }
}
