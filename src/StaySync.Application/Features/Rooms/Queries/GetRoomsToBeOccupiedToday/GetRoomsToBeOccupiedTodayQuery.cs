using StaySync.Application.Interfaces;

namespace StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;

public sealed record GetRoomsToBeOccupiedTodayQuery(DateOnly? DateOverride = null) : IQuery<TodayOccupancyDto>;
