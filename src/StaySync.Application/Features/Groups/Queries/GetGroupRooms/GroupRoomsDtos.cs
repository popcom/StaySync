namespace StaySync.Application.Features.Groups.Queries.GetGroupRooms;

public sealed record GroupTravellerDto(string Surname, string FirstName, DateOnly DateOfBirth);
public sealed record GroupRoomOccupancyDto(string RoomCode, int BedCount, IReadOnlyList<GroupTravellerDto> TravellersForDate);
public sealed record GroupRoomsDto(string GroupId, DateOnly? Date, IReadOnlyList<GroupRoomOccupancyDto> Rooms, IReadOnlyList<GroupTravellerDto> UnassignedTravellersForDate);
