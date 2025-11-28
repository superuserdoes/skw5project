namespace DeliFHery.Domain;

public class DeliveryOrder
{
    public DeliveryOrder(
        Guid id,
        string orderNumber,
        DateTimeOffset scheduledAt,
        DeliveryStatus status,
        Guid customerId,
        decimal weightKg = 0,
        decimal lengthCm = 0,
        decimal widthCm = 0,
        decimal heightCm = 0,
        string? originPostalCode = null,
        string? destinationPostalCode = null)
    {
        Id = id;
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        ScheduledAt = scheduledAt;
        Status = status;
        CustomerId = customerId;
        WeightKg = weightKg;
        LengthCm = lengthCm;
        WidthCm = widthCm;
        HeightCm = heightCm;
        OriginPostalCode = originPostalCode ?? string.Empty;
        DestinationPostalCode = destinationPostalCode ?? string.Empty;
    }

    public Guid Id { get; set; }

    public string OrderNumber { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public DeliveryStatus Status { get; set; }

    public Guid CustomerId { get; set; }

    public decimal WeightKg { get; set; }

    public decimal LengthCm { get; set; }

    public decimal WidthCm { get; set; }

    public decimal HeightCm { get; set; }

    public string OriginPostalCode { get; set; } = string.Empty;

    public string DestinationPostalCode { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public decimal DistanceSurcharge { get; set; }

    public decimal SeasonalAdjustment { get; set; }

    public decimal TotalPrice { get; set; }
}
