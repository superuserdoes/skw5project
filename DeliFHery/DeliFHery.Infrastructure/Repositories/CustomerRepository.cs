using System.Data;
using System.Linq;
using DeliFHery.Domain.Entities;
using DeliFHery.Infrastructure.Data;

namespace DeliFHery.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IContactMethodRepository _contactMethodRepository;

    public CustomerRepository(IDbConnectionFactory connectionFactory, IContactMethodRepository contactMethodRepository)
    {
        _connectionFactory = connectionFactory;
        _contactMethodRepository = contactMethodRepository;
    }

    public Customer? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Id, IdentitySubjectId, DisplayName, CreatedAt, UpdatedAt FROM Customers WHERE Id = @id";
        AddParameter(command, "@id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var customer = MapCustomer(reader);
        customer.ContactMethods = _contactMethodRepository.GetByCustomerId(customer.Id).ToList();
        return customer;
    }

    private static Customer MapCustomer(IDataRecord record)
    {
        return new Customer
        {
            Id = record.GetInt32(0),
            IdentitySubjectId = record.GetString(1),
            DisplayName = record.GetString(2),
            CreatedAt = record.GetDateTime(3),
            UpdatedAt = record.GetDateTime(4)
        };
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
