using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using Testcontainers.MsSql;

namespace StaySync.IntegrationTests.Infra;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;
    private string? _baseConnectionString; // server-level, no InitialCatalog

    public async Task InitializeAsync()
    {
        var isArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        _container = new MsSqlBuilder()
            .WithPassword("Your_password123")
            .WithImage(isArm ? "mcr.microsoft.com/azure-sql-edge:latest"
                             : "mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();
        _baseConnectionString = _container.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    /// <summary>Create a unique DB and return a connection string pointing to it.</summary>
    public async Task<string> CreateUniqueDatabaseAsync()
    {
        if (_baseConnectionString is null)
            throw new InvalidOperationException("Container not started.");

        var dbName = $"StaySync_{Guid.NewGuid():N}";
        var csb = new SqlConnectionStringBuilder(_baseConnectionString)
        {
            InitialCatalog = "master",
            TrustServerCertificate = true,
            Encrypt = false
        };

        await using (var conn = new SqlConnection(csb.ConnectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"IF DB_ID(@db) IS NULL CREATE DATABASE [{dbName}]";
            var p = cmd.CreateParameter();
            p.ParameterName = "@db";
            p.DbType = DbType.String;
            p.Value = dbName;
            cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }

        csb.InitialCatalog = dbName;
        return csb.ConnectionString;
    }

    public static async Task ExecAsync(string connectionString, string sql)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }
}
