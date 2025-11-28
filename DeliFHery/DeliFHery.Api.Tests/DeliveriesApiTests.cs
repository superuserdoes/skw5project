using System.Net;
using System.Net.Http.Json;
using DeliFHery.Api.Dtos;
using FluentAssertions;
using Xunit;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace DeliFHery.Api.Tests;

public class DeliveriesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid OrchardCustomerId = Guid.Parse("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10");

    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public DeliveriesApiTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task CalculatePrice_ReturnsBreakdown()
    {
        var request = BuildRequest(scheduleMonth: 12, origin: "10000", destination: "10001", weight: 6);

        var response = await _client.PostAsJsonAsync("/api/pricing/calculate", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadAsStringAsync();
        _output.WriteLine(payload);
        var quote = JsonSerializer.Deserialize<PriceQuoteDto>(payload, SerializerOptions);
        quote.Should().NotBeNull("Response payload: {0}", payload);
        quote!.BasePrice.Should().Be(15);
        quote.DistanceSurcharge.Should().Be(2.5m);
        quote.SeasonalAdjustment.Should().Be(2.63m);
        quote.TotalPrice.Should().BeApproximately(20.13m, 0.001m);
    }

    [Fact]
    public async Task CreateDelivery_ReturnsComputedPrice()
    {
        var request = BuildRequest(orderNumber: "ORD-API-NEW", weight: 3.5m, origin: "94010", destination: "94011");

        var response = await _client.PostAsJsonAsync($"/api/customers/{OrchardCustomerId}/deliveries", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadAsStringAsync();
        _output.WriteLine(payload);
        var delivery = JsonSerializer.Deserialize<DeliveryDto>(payload, SerializerOptions);
        delivery.Should().NotBeNull("Response payload: {0}", payload);
        delivery!.BasePrice.Should().Be(10);
        delivery.DistanceSurcharge.Should().Be(2.5m);
        delivery.SeasonalAdjustment.Should().Be(1.0m);
        delivery.TotalPrice.Should().Be(13.5m);
    }

    [Fact]
    public async Task CreateDelivery_InvalidParcelData_ReturnsBadRequest()
    {
        var request = BuildRequest(weight: 0, orderNumber: "ORD-INVALID");

        var response = await _client.PostAsJsonAsync($"/api/customers/{OrchardCustomerId}/deliveries", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static DeliveryRequest BuildRequest(
        string orderNumber = "ORD-QUOTE",
        decimal weight = 6,
        decimal length = 40,
        decimal width = 30,
        decimal height = 20,
        int scheduleMonth = 11,
        string origin = "10000",
        string destination = "10001")
    {
        return new DeliveryRequest
        {
            OrderNumber = orderNumber,
            ScheduledAt = new DateTimeOffset(new DateTime(2025, scheduleMonth, 15), TimeSpan.Zero),
            Status = DeliFHery.Domain.DeliveryStatus.Created,
            WeightKg = weight,
            LengthCm = length,
            WidthCm = width,
            HeightCm = height,
            OriginPostalCode = origin,
            DestinationPostalCode = destination
        };
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };
}
