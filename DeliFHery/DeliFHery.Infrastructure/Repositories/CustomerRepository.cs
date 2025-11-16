using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using DeliFHery.Domain.Entities;

namespace DeliFHery.Infrastructure.Repositories;

public class CustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Customer? GetById(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, IdentitySubjectId, DisplayName, CreatedAt, UpdatedAt FROM Customers WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new Customer
        {
            Id = reader.GetInt32(0),
            IdentitySubjectId = reader.GetString(1),
            DisplayName = reader.GetString(2),
            CreatedAt = reader.GetDateTime(3),
            UpdatedAt = reader.GetDateTime(4)
        };
    }
}

