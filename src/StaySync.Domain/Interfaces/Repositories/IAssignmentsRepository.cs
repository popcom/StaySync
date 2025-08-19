using StaySync.Domain.Entities;

namespace StaySync.Domain.Interfaces.Repositories;

public interface IAssignmentsRepository
{
    Task<RoomAssignment?> GetForTravellerOnDateAsync(Guid hotelId, Guid travellerId, DateOnly date, CancellationToken ct = default);
    Task<int> CountInRoomOnDateAsync(Guid roomId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(RoomAssignment assignment, CancellationToken ct = default);
    Task RemoveAsync(RoomAssignment assignment, CancellationToken ct = default);
}
