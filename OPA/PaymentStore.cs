namespace OPA;

public class PaymentStore
{
    private readonly Dictionary<string, Payment> _payments = new();

    public bool TryAddPayment(Payment payment)
    {
        if (_payments.ContainsKey(payment.PaymentId))
        {
            return false;
        }

        _payments[payment.PaymentId] = payment;
        return true;
    }

    public Payment? GetPayment(string paymentId)
    {
        _payments.TryGetValue(paymentId, out var payment);
        return payment;
    }

    public bool UpdatePayment(string paymentId, PaymentStatus status, string? failureReason = null)
    {
        if (_payments.TryGetValue(paymentId, out var payment))
        {
            payment.Status = status;
            payment.FailureReason = failureReason;
            return true;
        }
        return false;
    }
}
