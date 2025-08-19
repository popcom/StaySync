using System;
using FluentAssertions;
using StaySync.Domain.Entities;
using Xunit;

namespace StaySync.UnitTests.Domain.Entities;

public class RoomAssignmentTests
{
    [Fact]
    public void Reassign_updates_room()
    {
        var a = new RoomAssignment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2025, 8, 14));
        var target = Guid.NewGuid();
        a.ReassignToRoom(target);
        a.RoomId.Should().Be(target);
    }

    [Fact]
    public void Reassign_requires_target()
    {
        var a = new RoomAssignment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2025, 8, 14));
        var act = () => a.ReassignToRoom(default);
        act.Should().Throw<ArgumentException>();
    }
}
