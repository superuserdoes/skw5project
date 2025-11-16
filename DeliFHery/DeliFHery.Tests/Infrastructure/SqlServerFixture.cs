using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DeliFHery.Domain.Entities;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace DeliFHery.Tests.Infrastructure;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;

    public SqlServerFixture()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .WithPassword("yourStrong(!)Password")
            .Build();
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await InitializeSchemaAsync();
    }

    private async Task InitializeSchemaAsync()
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Data", "SqlSchema.sql");
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Unable to locate schema file at '{schemaPath}'.");
        }

        var schemaSql = await File.ReadAllTextAsync(schemaPath);

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        foreach (var batch in SplitBatches(schemaSql))
        {
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = batch;
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static IEnumerable<string> SplitBatches(string sql)
    {
        return Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    public async Task ResetDatabaseAsync()
    {
        const string sql = """
        DELETE FROM ContactMethods;
        DELETE FROM Customers;
        DBCC CHECKIDENT ('dbo.ContactMethods', RESEED, 0);
        DBCC CHECKIDENT ('dbo.Customers', RESEED, 0);
        """;

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertCustomerAsync(Customer customer)
    {
        const string sql = """
        INSERT INTO Customers (IdentitySubjectId, DisplayName, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@IdentitySubjectId, @DisplayName, @CreatedAt, @UpdatedAt);
        """;

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("@IdentitySubjectId", customer.IdentitySubjectId);
        cmd.Parameters.AddWithValue("@DisplayName", customer.DisplayName);
        cmd.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);
        cmd.Parameters.AddWithValue("@UpdatedAt", customer.UpdatedAt);

        var insertedId = await cmd.ExecuteScalarAsync();
        if (insertedId is null)
        {
            throw new InvalidOperationException("Failed to insert customer test data.");
        }

        return Convert.ToInt32(insertedId);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
