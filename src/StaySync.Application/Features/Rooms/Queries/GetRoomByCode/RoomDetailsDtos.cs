namespace StaySync.Application.Features.Rooms.Queries.GetRoomByCode;

public sealed record RoomDetailsTravellerDto(string Surname, string FirstName, DateOnly DateOfBirth, string GroupId);
public sealed record RoomDetailsDto(string RoomCode, int BedCount, IReadOnlyList<RoomDetailsTravellerDto> Travellers, DateOnly Date);
