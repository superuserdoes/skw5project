using DeliFHery.Domain;
using DeliFHery.Logic.Pricing;
using DeliFHery.Logic.Pricing.Rules;
using FluentAssertions;
using Xunit;

namespace DeliFHery.Logic.Tests;

public class PricingServiceTests
{
    private static IPricingService BuildService()
    {
        var rules = new IPriceRule[]
        {
            new WeightPriceRule(),
            new DistanceSurchargeRule(),
            new SeasonalAdjustmentRule()
        };

        return new PricingService(rules);
    }

    [Theory]
    [InlineData(0.5, 5)]
    [InlineData(3, 10)]
    [InlineData(8, 15)]
    [InlineData(15, 25)]
    [InlineData(25, 40)]
    public void WeightRule_UsesTieredBasePrice(decimal weight, decimal expectedBase)
    {
        var order = CreateOrder(weightKg: weight);
        var breakdown = BuildService().Calculate(order);

        breakdown.BasePrice.Should().Be(expectedBase);
    }

    [Fact]
    public void WeightRule_UsesDimensionalWeightWhenHigher()
    {
        var order = CreateOrder(weightKg: 2, length: 200, width: 50, height: 30);

        var breakdown = BuildService().Calculate(order);

        breakdown.BasePrice.Should().Be(40);
    }

    [Fact]
    public void DistanceRule_AddsSteppedSurcharge()
    {
        var order = CreateOrder(origin: "10000", destination: "10050");

        var breakdown = BuildService().Calculate(order);

        breakdown.DistanceSurcharge.Should().Be(25m);
    }

    [Fact]
    public void DistanceRule_NoSurchargeForSamePostalCode()
    {
        var order = CreateOrder(origin: "90210", destination: "90210");

        var breakdown = BuildService().Calculate(order);

        breakdown.DistanceSurcharge.Should().Be(0);
    }

    [Fact]
    public void SeasonalRule_AddsWinterAdjustment()
    {
        var order = CreateOrder(scheduleMonth: 12, origin: "10000", destination: "10050");

        var breakdown = BuildService().Calculate(order);

        breakdown.SeasonalAdjustment.Should().Be((breakdown.BasePrice + breakdown.DistanceSurcharge) * 0.15m);
        breakdown.TotalPrice.Should().Be(breakdown.BasePrice + breakdown.DistanceSurcharge + breakdown.SeasonalAdjustment);
    }

    private static DeliveryOrder CreateOrder(
        decimal weightKg = 5,
        decimal length = 30,
        decimal width = 10,
        decimal height = 10,
        string origin = "94103",
        string destination = "94107",
        int scheduleMonth = 1)
    {
        return new DeliveryOrder(
            Guid.NewGuid(),
            "ORD-TEST",
            new DateTimeOffset(new DateTime(2025, scheduleMonth, 15), TimeSpan.Zero),
            DeliveryStatus.Created,
            Guid.NewGuid(),
            weightKg,
            length,
            width,
            height,
            origin,
            destination);
    }
}
