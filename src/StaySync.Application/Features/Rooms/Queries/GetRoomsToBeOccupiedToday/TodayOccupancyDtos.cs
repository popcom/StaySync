namespace StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;

public sealed record TravellerLiteDto(string Surname, string FirstName, DateOnly DateOfBirth, string GroupId);

public sealed record RoomOccupancyDto(string RoomCode, int BedCount, IReadOnlyList<TravellerLiteDto> Travellers);

public sealed record TodayOccupancyDto(DateOnly Date, IReadOnlyList<RoomOccupancyDto> Rooms);
