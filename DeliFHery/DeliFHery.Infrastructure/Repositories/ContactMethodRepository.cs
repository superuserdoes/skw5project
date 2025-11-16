using System;
using System.Collections.Generic;
using System.Data;
using DeliFHery.Domain.Entities;
using DeliFHery.Infrastructure.Data;

namespace DeliFHery.Infrastructure.Repositories;

public class ContactMethodRepository : IContactMethodRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ContactMethodRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<ContactMethod> GetByCustomerId(int customerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Id, CustomerId, Type, Value, IsPrimary, IsVerified, CreatedAt, VerifiedAt FROM ContactMethods WHERE CustomerId = @customerId ORDER BY Id";
        AddParameter(command, "@customerId", customerId);

        using var reader = command.ExecuteReader();
        var results = new List<ContactMethod>();

        while (reader.Read())
        {
            results.Add(MapContactMethod(reader));
        }

        return results;
    }

    public ContactMethod Add(ContactMethod contactMethod)
    {
        var now = DateTime.UtcNow;
        contactMethod.CreatedAt = contactMethod.CreatedAt == default ? now : contactMethod.CreatedAt;

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            @"INSERT INTO ContactMethods (CustomerId, Type, Value, IsPrimary, IsVerified, CreatedAt, VerifiedAt)
              VALUES (@customerId, @type, @value, @isPrimary, @isVerified, @createdAt, @verifiedAt);";

        AddParameter(command, "@customerId", contactMethod.CustomerId);
        AddParameter(command, "@type", contactMethod.Type);
        AddParameter(command, "@value", contactMethod.Value);
        AddParameter(command, "@isPrimary", contactMethod.IsPrimary);
        AddParameter(command, "@isVerified", contactMethod.IsVerified);
        AddParameter(command, "@createdAt", contactMethod.CreatedAt);
        AddParameter(command, "@verifiedAt", contactMethod.VerifiedAt);

        command.ExecuteNonQuery();

        using var identityCommand = connection.CreateCommand();
        identityCommand.CommandText = BuildIdentityQuery(connection);
        var newId = Convert.ToInt32(identityCommand.ExecuteScalar());
        contactMethod.Id = newId;

        return contactMethod;
    }

    public void Update(ContactMethod contactMethod)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            @"UPDATE ContactMethods
               SET Type = @type,
                   Value = @value,
                   IsPrimary = @isPrimary,
                   IsVerified = @isVerified,
                   VerifiedAt = @verifiedAt
             WHERE Id = @id";

        AddParameter(command, "@type", contactMethod.Type);
        AddParameter(command, "@value", contactMethod.Value);
        AddParameter(command, "@isPrimary", contactMethod.IsPrimary);
        AddParameter(command, "@isVerified", contactMethod.IsVerified);
        AddParameter(command, "@verifiedAt", contactMethod.VerifiedAt);
        AddParameter(command, "@id", contactMethod.Id);

        command.ExecuteNonQuery();
    }

    private static ContactMethod MapContactMethod(IDataRecord record)
    {
        return new ContactMethod
        {
            Id = record.GetInt32(0),
            CustomerId = record.GetInt32(1),
            Type = record.GetString(2),
            Value = record.GetString(3),
            IsPrimary = record.GetBoolean(4),
            IsVerified = record.GetBoolean(5),
            CreatedAt = record.GetDateTime(6),
            VerifiedAt = record.IsDBNull(7) ? null : record.GetDateTime(7)
        };
    }

    private static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static string BuildIdentityQuery(IDbConnection connection)
    {
        var providerName = connection.GetType().Name;
        return providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase)
            ? "SELECT last_insert_rowid();"
            : "SELECT CAST(SCOPE_IDENTITY() as int);";
    }
}
