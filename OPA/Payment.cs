namespace OPA;

public class Payment
{
    public required string PaymentId { get; set; }
    public required string ApiKey { get; set; }
    public required decimal Amount { get; set; }
    public required string CallbackUrl { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
