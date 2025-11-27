namespace OPA;

public class CreatePaymentRequest
{
    public required string PaymentId { get; set; }
    public required decimal Amount { get; set; }
    public required string CallbackUrl { get; set; }
    public string? RedirectUrl { get; set; }
}
