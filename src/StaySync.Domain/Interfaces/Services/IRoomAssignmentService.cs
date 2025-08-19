using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Interfaces.Services;

public interface IRoomAssignmentService
{
    Task MoveTravellerAsync(
        Guid hotelId,
        GroupId groupId,
        string surname,
        string firstName,
        DateOnly dateOfBirth,
        RoomCode from,
        RoomCode to,
        DateOnly assignedOnDate,
        CancellationToken ct = default);
}
