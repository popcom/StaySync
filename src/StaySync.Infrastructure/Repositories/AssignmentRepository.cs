using Microsoft.EntityFrameworkCore;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.Repositories;

public sealed class AssignmentsRepository(StaySyncDbContext db) : IAssignmentsRepository
{
    public Task<RoomAssignment?> GetForTravellerOnDateAsync(Guid hotelId, Guid travellerId, DateOnly date, CancellationToken ct = default)
        => db.RoomAssignments.FirstOrDefaultAsync(a =>
            a.HotelId == hotelId && a.TravellerId == travellerId && a.AssignedOnDate == date, ct);

    public Task<int> CountInRoomOnDateAsync(Guid roomId, DateOnly date, CancellationToken ct = default)
        => db.RoomAssignments.CountAsync(a => a.RoomId == roomId && a.AssignedOnDate == date, ct);

    public Task AddAsync(RoomAssignment assignment, CancellationToken ct = default)
        => db.RoomAssignments.AddAsync(assignment, ct).AsTask();

    public Task RemoveAsync(RoomAssignment assignment, CancellationToken ct = default)
    {
        db.RoomAssignments.Remove(assignment);
        return Task.CompletedTask;
    }
}
