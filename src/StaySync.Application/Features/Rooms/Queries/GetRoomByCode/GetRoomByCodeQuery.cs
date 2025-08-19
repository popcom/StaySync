using StaySync.Application.Interfaces;

namespace StaySync.Application.Features.Rooms.Queries.GetRoomByCode;

public sealed record GetRoomByCodeQuery(string RoomCode, DateOnly? DateOverride = null)
    : IQuery<RoomDetailsDto>;
