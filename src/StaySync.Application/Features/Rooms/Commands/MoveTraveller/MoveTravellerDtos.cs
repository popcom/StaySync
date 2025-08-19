namespace StaySync.Application.Features.Rooms.Commands.MoveTraveller;

public sealed record MoveTravellerRequest(
    string GroupId,
    string Surname,
    string FirstName,
    DateOnly DateOfBirth,
    string FromRoomCode,
    string ToRoomCode,
    DateOnly AssignedOnDate);

public sealed record RoomSnapshotTravellerDto(string Surname, string FirstName, DateOnly DateOfBirth, string GroupId);
public sealed record RoomSnapshotDto(string RoomCode, int BedCount, IReadOnlyList<RoomSnapshotTravellerDto> Travellers);

public sealed record MoveTravellerResult(
    DateOnly Date,
    RoomSnapshotDto FromRoomAfter,
    RoomSnapshotDto ToRoomAfter);
