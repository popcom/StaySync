using StaySync.Domain.Interfaces;
using StaySync.Infrastructure.Persistence;

namespace StaySync.Infrastructure.UnitOfWork;

public sealed class EfUnitOfWork(StaySyncDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
