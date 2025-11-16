using DeliFHery.Domain.Entities;
using DeliFHery.Infrastructure.Repositories;
using DeliFHery.Tests.Infrastructure;

namespace DeliFHery.Tests;

public class CustomerRepositoryTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
        _repository = new CustomerRepository(_fixture.ConnectionString);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotExists()
    {
        await _fixture.ResetDatabaseAsync();

        var result = _repository.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ReturnsCustomer_WhenExists()
    {
        await _fixture.ResetDatabaseAsync();
        var now = DateTime.UtcNow;
        var newCustomer = new Customer
        {
            IdentitySubjectId = Guid.NewGuid().ToString(),
            DisplayName = "Integration Test Customer",
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _fixture.InsertCustomerAsync(newCustomer);

        var result = _repository.GetById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal(newCustomer.IdentitySubjectId, result.IdentitySubjectId);
        Assert.Equal(newCustomer.DisplayName, result.DisplayName);
    }
}
