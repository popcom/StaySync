using FluentAssertions;
using Moq;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Interfaces;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Entities;
using StaySync.Domain.Exceptions;
using StaySync.Domain.Interfaces;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Domain.ValueObjects;
using Xunit;

namespace StaySync.UnitTests.Application.Handlers.Rooms;

public class MoveTravellerHandlerTests
{
    private static readonly Guid HotelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static MoveTravellerHandler CreateHandler(
        out Mock<ITravelGroupRepository> groups,
        out Mock<ITravellerRepository> travellers,
        out Mock<IRoomRepository> rooms,
        out Mock<IAssignmentsRepository> assignments,
        out Mock<IUnitOfWork> uow,
        out Mock<IRoomQueries> roomQueries)
    {
        var current = new Mock<ICurrentHotelContext>();
        current.SetupGet(x => x.HotelId).Returns(HotelId);

        groups = new Mock<ITravelGroupRepository>();
        travellers = new Mock<ITravellerRepository>();
        rooms = new Mock<IRoomRepository>();
        assignments = new Mock<IAssignmentsRepository>();
        uow = new Mock<IUnitOfWork>();
        roomQueries = new Mock<IRoomQueries>();

        return new MoveTravellerHandler(
            current.Object, roomQueries.Object, rooms.Object, groups.Object,
            travellers.Object, assignments.Object, uow.Object);
    }

    private static MoveTravellerCommand ValidCommand() =>
        new(new MoveTravellerRequest(
            GroupId: "A12B34",
            Surname: "Doe",
            FirstName: "John",
            DateOfBirth: new DateOnly(1988, 3, 2),
            FromRoomCode: "0101",
            ToRoomCode: "0102",
            AssignedOnDate: new DateOnly(2025, 8, 14)));

    [Fact]
    public async Task Group_not_found_throws()
    {
        var handler = CreateHandler(out var groups, out _, out _, out _, out _, out _);
        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync((TravelGroup?)null);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*group*");
    }

    [Fact]
    public async Task Traveller_not_found_throws()
    {
        var handler = CreateHandler(out var groups, out var travellers, out _, out _, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);

        travellers.Setup(t => t.GetByIdentityAsync(group.Id, new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2)), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Traveller?)null);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*Traveller*");
    }

    [Fact]
    public async Task From_room_not_found_throws()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out _, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id, travellerKey);
        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);

        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Room?)null);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*From room*");
    }

    [Fact]
    public async Task To_room_not_found_throws()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out _, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id, travellerKey);
        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);

        var fromRoom = new Room(HotelId, new RoomCode("0101"), 2);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>())).ReturnsAsync(fromRoom);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0102"), It.IsAny<CancellationToken>())).ReturnsAsync((Room?)null);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*To room*");
    }

    [Fact]
    public async Task Not_assigned_on_date_throws()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out var assignments, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id,travellerKey);
        var fromRoom = new Room(HotelId, new RoomCode("0101"), 2);
        var toRoom = new Room(HotelId, new RoomCode("0102"), 2);

        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>())).ReturnsAsync(fromRoom);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0102"), It.IsAny<CancellationToken>())).ReturnsAsync(toRoom);

        assignments.Setup(a => a.GetForTravellerOnDateAsync(HotelId, trav.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync((RoomAssignment?)null);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*not assigned*");
    }

    [Fact]
    public async Task From_room_mismatch_throws_conflict()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out var assignments, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id, travellerKey);
        var fromRoom = new Room(HotelId, new RoomCode("0101"), 2);
        var toRoom = new Room(HotelId, new RoomCode("0102"), 2);

        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>())).ReturnsAsync(fromRoom);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0102"), It.IsAny<CancellationToken>())).ReturnsAsync(toRoom);

        var assignment = new RoomAssignment(HotelId, Guid.NewGuid(), trav.Id, new DateOnly(2025, 8, 14)); // wrong room id
        assignments.Setup(a => a.GetForTravellerOnDateAsync(HotelId, trav.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(assignment);

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<ConflictException>().WithMessage("*from*");
    }

    [Fact]
    public async Task Capacity_full_throws_conflict()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out var assignments, out _, out _);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id, travellerKey);
        var fromRoom = new Room(HotelId, new RoomCode("0101"), 2);
        var toRoom = new Room(HotelId, new RoomCode("0102"), 2);

        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>())).ReturnsAsync(fromRoom);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0102"), It.IsAny<CancellationToken>())).ReturnsAsync(toRoom);

        var assignment = new RoomAssignment(HotelId, fromRoom.Id, trav.Id, new DateOnly(2025, 8, 14));
        assignments.Setup(a => a.GetForTravellerOnDateAsync(HotelId, trav.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(assignment);

        assignments.Setup(a => a.CountInRoomOnDateAsync(toRoom.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(toRoom.BedCount); // full

        var act = () => handler.Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<ConflictException>().WithMessage("*over-occupied*");
    }

    [Fact]
    public async Task Success_moves_and_returns_snapshots()
    {
        var handler = CreateHandler(out var groups, out var travellers, out var rooms, out var assignments, out var uow, out var roomQueries);

        var group = new TravelGroup(HotelId, new GroupId("A12B34"), new DateOnly(2025, 8, 14), 1);
        var travellerKey = new TravellerKey("DOE", "JOHN", new DateOnly(1988, 3, 2));
        var trav = new Traveller(group.Id, travellerKey);
        var fromRoom = new Room(HotelId, new RoomCode("0101"), 2);
        var toRoom = new Room(HotelId, new RoomCode("0102"), 2);

        groups.Setup(g => g.GetByGroupIdAsync(HotelId, It.IsAny<GroupId>(), It.IsAny<CancellationToken>())).ReturnsAsync(group);
        travellers.Setup(t => t.GetByIdentityAsync(group.Id, travellerKey, It.IsAny<CancellationToken>())).ReturnsAsync(trav);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0101"), It.IsAny<CancellationToken>())).ReturnsAsync(fromRoom);
        rooms.Setup(r => r.GetByCodeAsync(HotelId, new RoomCode("0102"), It.IsAny<CancellationToken>())).ReturnsAsync(toRoom);

        var assignment = new RoomAssignment(HotelId, fromRoom.Id, trav.Id, new DateOnly(2025, 8, 14));
        assignments.Setup(a => a.GetForTravellerOnDateAsync(HotelId, trav.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(assignment);

        assignments.Setup(a => a.CountInRoomOnDateAsync(toRoom.Id, new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(toRoom.BedCount - 1);

        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var fromSnap = new RoomDetailsDto("0101", 2, [], new DateOnly(2025, 8, 14));
        var toSnap = new RoomDetailsDto("0102", 2, [], new DateOnly(2025, 8, 14));
        roomQueries.Setup(q => q.GetRoomByCodeAsync(HotelId, "0101", new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>())).ReturnsAsync(fromSnap);
        roomQueries.Setup(q => q.GetRoomByCodeAsync(HotelId, "0102", new DateOnly(2025, 8, 14), It.IsAny<CancellationToken>())).ReturnsAsync(toSnap);

        var result = await handler.Handle(ValidCommand(), default);

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.FromRoomAfter.RoomCode.Should().Be("0101");
        result.ToRoomAfter.RoomCode.Should().Be("0102");
    }
}
