using System.Net;
using System.Net.Http.Json;
using DeliFHery.Api.Dtos;
using FluentAssertions;
using Xunit;

namespace DeliFHery.Api.Tests;

public class CustomersApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomersApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSeededCustomers()
    {
        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
        customers!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedCustomer()
    {
        var request = new CustomerRequest
        {
            Name = "Integration Test Store",
            Street = "Integration Street 1",
            City = "Test City",
            PostalCode = "12345"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be(request.Name);
        customer.Street.Should().Be(request.Street);
    }
}
