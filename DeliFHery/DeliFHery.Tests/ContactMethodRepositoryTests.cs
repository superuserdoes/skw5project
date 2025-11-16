using System;
using System.Linq;
using DeliFHery.Domain.Entities;
using DeliFHery.Infrastructure.Repositories;

namespace DeliFHery.Tests;

public class ContactMethodRepositoryTests
{
    [Fact]
    public void GetByCustomerId_ReturnsContactMethods()
    {
        using var database = new SqliteTestDatabase();
        database.InsertCustomer(1, "subject-1", "Alice", DateTime.UtcNow, DateTime.UtcNow);
        database.InsertContactMethod(10, 1, "Email", "alice@example.com", true, true, DateTime.UtcNow, DateTime.UtcNow);
        database.InsertContactMethod(11, 1, "Phone", "+123456789", false, false, DateTime.UtcNow, null);

        var repository = new ContactMethodRepository(database.ConnectionFactory);

        var methods = repository.GetByCustomerId(1).ToList();

        Assert.Equal(2, methods.Count);
        Assert.Contains(methods, method => method.Type == "Email" && method.Value == "alice@example.com");
        Assert.Contains(methods, method => method.Type == "Phone" && method.Value == "+123456789");
    }

    [Fact]
    public void Add_AssignsIdentifierAndPersistsData()
    {
        using var database = new SqliteTestDatabase();
        database.InsertCustomer(1, "subject-1", "Alice", DateTime.UtcNow, DateTime.UtcNow);
        var repository = new ContactMethodRepository(database.ConnectionFactory);
        var contactMethod = new ContactMethod
        {
            CustomerId = 1,
            Type = "Email",
            Value = "alice@example.com",
            IsPrimary = true,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        var inserted = repository.Add(contactMethod);

        Assert.True(inserted.Id > 0);
        var storedValue = database.QueryScalar<string>("SELECT Value FROM ContactMethods WHERE Id = @Id", ("@Id", inserted.Id));
        Assert.Equal("alice@example.com", storedValue);
    }

    [Fact]
    public void Update_PersistsChanges()
    {
        using var database = new SqliteTestDatabase();
        database.InsertCustomer(1, "subject-1", "Alice", DateTime.UtcNow, DateTime.UtcNow);
        database.InsertContactMethod(20, 1, "Email", "alice@example.com", true, false, DateTime.UtcNow, null);
        var repository = new ContactMethodRepository(database.ConnectionFactory);
        var updated = new ContactMethod
        {
            Id = 20,
            CustomerId = 1,
            Type = "Phone",
            Value = "+987654321",
            IsPrimary = false,
            IsVerified = true,
            VerifiedAt = DateTime.UtcNow
        };

        repository.Update(updated);

        var storedType = database.QueryScalar<string>("SELECT Type FROM ContactMethods WHERE Id = @Id", ("@Id", 20));
        var storedValue = database.QueryScalar<string>("SELECT Value FROM ContactMethods WHERE Id = @Id", ("@Id", 20));
        Assert.Equal("Phone", storedType);
        Assert.Equal("+987654321", storedValue);
    }
}
