using System;
using DeliFHery.Infrastructure.Repositories;
using DeliFHery.Tests.Infrastructure;

namespace DeliFHery.Tests;

namespace DeliFHery.Tests;

public class CustomerRepositoryTests
{
    [Fact]
    public async Task GetById_ReturnsNull_WhenNotExists()
    {
        using var database = new SqliteTestDatabase();
        var contactRepository = new ContactMethodRepository(database.ConnectionFactory);
        var repository = new CustomerRepository(database.ConnectionFactory, contactRepository);

        var result = repository.GetById(99);

        Assert.Null(result);
    }

    [Fact]
    public void GetById_ReturnsCustomerWithContactMethods()
    {
        using var database = new SqliteTestDatabase();
        var timestamp = DateTime.UtcNow;
        database.InsertCustomer(1, "subject-1", "Alice", timestamp, timestamp);
        database.InsertContactMethod(100, 1, "Email", "alice@example.com", true, true, timestamp, timestamp);
        var contactRepository = new ContactMethodRepository(database.ConnectionFactory);
        var repository = new CustomerRepository(database.ConnectionFactory, contactRepository);

        var customer = repository.GetById(1);

        Assert.NotNull(customer);
        Assert.Equal("Alice", customer!.DisplayName);
        Assert.Single(customer.ContactMethods);
        Assert.Equal("alice@example.com", customer.ContactMethods[0].Value);
    }
}
