using StaySync.Application.Interfaces;

namespace StaySync.Application.Features.Groups.Queries.GetGroupRooms;

public sealed record GetGroupRoomsQuery(string GroupId, DateOnly? DateOverride = null)
    : IQuery<GroupRoomsDto>;
