using System;
using FluentAssertions;
using StaySync.Domain.Entities;
using StaySync.Domain.ValueObjects;
using Xunit;

namespace StaySync.UnitTests.Domain.Entities;

public class RoomTests
{
    [Fact]
    public void Creates_room()
    {
        var room = new Room(Guid.NewGuid(), new RoomCode("0101"), 2);
        room.BedCount.Should().Be(2);
        room.RoomCode.Value.Should().Be("0101");
    }

    [Fact]
    public void BedCount_must_be_positive()
    {
        var act = () => new Room(Guid.NewGuid(), new RoomCode("0101"), 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void HotelId_required()
    {
        var act = () => new Room(default, new RoomCode("0101"), 2);
        act.Should().Throw<ArgumentException>();
    }
}
