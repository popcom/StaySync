using System.Data;
using Microsoft.Data.SqlClient;

namespace StaySync.Infrastructure.Read;

internal interface ISqlConnectionFactory
{
    IDbConnection Create();
}

internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection Create() => new SqlConnection(connectionString);
}
