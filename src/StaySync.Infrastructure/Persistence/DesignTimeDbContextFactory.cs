using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StaySync.Infrastructure.Persistence;


/// This factory is used by EF Core tools to create a DbContext instance at design time without start up project.
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StaySyncDbContext>
{
    public StaySyncDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<StaySyncDbContext>();

        /// Connection string is expected to be set in the environment variable STAYSYNC_EF_CONN
        /// set STAYSYNC_EF_CONN = "Server=sql,1433;Database=StaySync;User Id=sa;Password=...;Encrypt=False;TrustServerCertificate=True"
        string conn = Environment.GetEnvironmentVariable("STAYSYNC_EF_CONN") ?? "";

        if (string.IsNullOrWhiteSpace(conn))
        {
            throw new InvalidOperationException("Connection string STAYSYNC_EF_CONN is not set.");
        }
        builder.UseSqlServer(conn, sql => sql.EnableRetryOnFailure(5));
        return new StaySyncDbContext(builder.Options);
    }
}
