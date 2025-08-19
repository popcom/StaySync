using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Interfaces;
using StaySync.Application.Interfaces.Read;
using StaySync.Domain.Exceptions;
using StaySync.Domain.Interfaces;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Domain.ValueObjects;

namespace StaySync.Application.Features.Rooms.Commands.MoveTraveller;

public sealed class MoveTravellerHandler(
    ICurrentHotelContext current,
    IRoomQueries roomQueries, // to return snapshots efficiently
    IRoomRepository rooms,
    ITravelGroupRepository groups,
    ITravellerRepository travellers,
    IAssignmentsRepository assignments,
    IUnitOfWork uow)
    : ICommandHandler<MoveTravellerCommand, MoveTravellerResult>
{
    public async Task<MoveTravellerResult> Handle(MoveTravellerCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;
        var hotelId = current.HotelId;

        // 1) Validate group exists
        var groupIdVo = new GroupId(req.GroupId);
        var group = await groups.GetByGroupIdAsync(hotelId, groupIdVo, ct)
                   ?? throw new NotFoundException("Travel group not found.");

        // 2) Resolve traveller by key
        var travellerKey = new TravellerKey(req.Surname, req.FirstName, req.DateOfBirth);
        var traveller = await travellers.GetByIdentityAsync(group.Id, travellerKey, ct)
                        ?? throw new NotFoundException("Traveller not found in this group.");

        // 3) Resolve rooms
        var fromCode = new RoomCode(req.FromRoomCode);
        var toCode = new RoomCode(req.ToRoomCode);

        var fromRoom = await rooms.GetByCodeAsync(hotelId, fromCode, ct)
                       ?? throw new NotFoundException("From room not found.");
        var toRoom = await rooms.GetByCodeAsync(hotelId, toCode, ct)
                     ?? throw new NotFoundException("To room not found.");

        // 4) Get current assignment for the date
        var assignment = await assignments.GetForTravellerOnDateAsync(hotelId, traveller.Id, req.AssignedOnDate, ct)
                         ?? throw new NotFoundException("Traveller is not assigned on the given date.");
        if (assignment.RoomId != fromRoom.Id)
            throw new ConflictException("Traveller is not in the specified 'from' room.");

        // 5) Capacity check on target room
        var toCount = await assignments.CountInRoomOnDateAsync(toRoom.Id, req.AssignedOnDate, ct);
        if (toCount >= toRoom.BedCount)
            throw new ConflictException("Target room would be over-occupied.");

        // 6) Perform move (EF change tracking)
        assignment.ReassignToRoom(toRoom.Id);
        await uow.SaveChangesAsync(ct);

        // 7) Return fresh snapshots for both rooms on the given date
        var fromSnap = await roomQueries.GetRoomByCodeAsync(hotelId, fromCode.Value, req.AssignedOnDate, ct)
                       ?? new RoomDetailsDto(fromCode.Value, fromRoom.BedCount, [], req.AssignedOnDate);

        var toSnap = await roomQueries.GetRoomByCodeAsync(hotelId, toCode.Value, req.AssignedOnDate, ct)
                     ?? new RoomDetailsDto(toCode.Value, toRoom.BedCount, [], req.AssignedOnDate);

        return new MoveTravellerResult(
            Date: req.AssignedOnDate,
            FromRoomAfter: new RoomSnapshotDto(
                fromSnap.RoomCode, fromSnap.BedCount,
                fromSnap.Travellers.Select(t =>
                    new RoomSnapshotTravellerDto(t.Surname, t.FirstName, t.DateOfBirth, t.GroupId)).ToList()),
            ToRoomAfter: new RoomSnapshotDto(
                toSnap.RoomCode, toSnap.BedCount,
                toSnap.Travellers.Select(t =>
                    new RoomSnapshotTravellerDto(t.Surname, t.FirstName, t.DateOfBirth, t.GroupId)).ToList())
        );
    }
}
