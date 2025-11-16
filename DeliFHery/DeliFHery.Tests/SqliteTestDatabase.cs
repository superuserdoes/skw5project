using System;
using System.Data;
using DeliFHery.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace DeliFHery.Tests;

internal sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbConnectionFactory _connectionFactory;

    public SqliteTestDatabase()
    {
        var connectionString = $"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared";
        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();
        _connectionFactory = new DelegateConnectionFactory(() => new SqliteConnection(connectionString));
        InitializeSchema();
    }

    public IDbConnectionFactory ConnectionFactory => _connectionFactory;

    public void InsertCustomer(int id, string identitySubjectId, string displayName, DateTime createdAt, DateTime updatedAt)
    {
        ExecuteNonQuery(
            @"INSERT INTO Customers (Id, IdentitySubjectId, DisplayName, CreatedAt, UpdatedAt)
              VALUES (@Id, @IdentitySubjectId, @DisplayName, @CreatedAt, @UpdatedAt);",
            ("@Id", id),
            ("@IdentitySubjectId", identitySubjectId),
            ("@DisplayName", displayName),
            ("@CreatedAt", createdAt),
            ("@UpdatedAt", updatedAt));
    }

    public void InsertContactMethod(int id, int customerId, string type, string value, bool isPrimary, bool isVerified,
        DateTime createdAt, DateTime? verifiedAt)
    {
        ExecuteNonQuery(
            @"INSERT INTO ContactMethods (Id, CustomerId, Type, Value, IsPrimary, IsVerified, CreatedAt, VerifiedAt)
              VALUES (@Id, @CustomerId, @Type, @Value, @IsPrimary, @IsVerified, @CreatedAt, @VerifiedAt);",
            ("@Id", id),
            ("@CustomerId", customerId),
            ("@Type", type),
            ("@Value", value),
            ("@IsPrimary", isPrimary),
            ("@IsVerified", isVerified),
            ("@CreatedAt", createdAt),
            ("@VerifiedAt", verifiedAt));
    }

    public T? QueryScalar<T>(string sql, params (string Name, object? Value)[] parameters)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        var result = command.ExecuteScalar();
        return result == null || result is DBNull ? default : (T?)Convert.ChangeType(result, typeof(T));
    }

    public void Dispose()
    {
        _keepAliveConnection.Dispose();
    }

    private void InitializeSchema()
    {
        ExecuteNonQuery(
            @"CREATE TABLE Customers (
                Id INTEGER PRIMARY KEY,
                IdentitySubjectId TEXT NOT NULL,
                DisplayName TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );");

        ExecuteNonQuery(
            @"CREATE TABLE ContactMethods (
                Id INTEGER PRIMARY KEY,
                CustomerId INTEGER NOT NULL,
                Type TEXT NOT NULL,
                Value TEXT NOT NULL,
                IsPrimary INTEGER NOT NULL,
                IsVerified INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                VerifiedAt TEXT NULL,
                FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );");
    }

    private void ExecuteNonQuery(string sql, params (string Name, object? Value)[] parameters)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(IDbCommand command, params (string Name, object? Value)[] parameters)
    {
        foreach (var (name, value) in parameters)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }

    private sealed class DelegateConnectionFactory : IDbConnectionFactory
    {
        private readonly Func<IDbConnection> _factory;

        public DelegateConnectionFactory(Func<IDbConnection> factory)
        {
            _factory = factory;
        }

        public IDbConnection CreateConnection()
        {
            return _factory();
        }
    }
}
