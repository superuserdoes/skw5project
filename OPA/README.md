# OPA (Online Payment Approval) - Demo Payment Provider

This is a demo implementation of a third-party payment provider for SWK 5. It provides a simple API for creating payments and a web interface for processing them.

## Configuration

Before using the OPA system, you must configure the API key in `appsettings.json`:

```json
{
  "OPA": {
    "ApiKey": "your-secure-api-key"
  }
}
```

**Important:** Only requests with the correct API key will be accepted. The default API key is `OPA-12345-67890`.

You can change this to any value you prefer for your environment.

## How It Works

### 1. Create a Payment

Send a POST request to `/api/payment/create`:

**Headers:**
```
X-API-Key: demo-api-key-12345
Content-Type: application/json
```

**Body:**
```json
{
  "paymentId": "unique-payment-id",
  "amount": 99.99,
  "callbackUrl": "https://your-system.com/payment-callback",
  "redirectUrl": "https://your-system.com/payment-complete"
}
```

**Note:** 
- `redirectUrl` is optional. If provided, the API will automatically append it as a query parameter to the payment URL.
- `paymentId` must be unique. If a payment with the same ID already exists, the API will return a 409 Conflict error.
- The `X-API-Key` header must match the configured API key in `appsettings.json`.

**Response (Success - 200 OK):**
```json
{
  "paymentUrl": "https://localhost:5001/payment/{paymentId}?redirect-url=https://your-system.com/payment-complete"
}
```

**Response (Unauthorized - 401):**
Returned when X-API-Key header is missing, empty, or does not match the configured API key.

**Response (Conflict - 409 Conflict):**
```json
{
  "error": "Payment ID already exists",
  "paymentId": "unique-payment-id"
}
```

### 2. Redirect Customer

Redirect your customer to the `paymentUrl` returned by the API.

**Two ways to specify redirect URL:**

1. **Include in API request** (recommended):
   ```json
   {
     "paymentId": "order-123",
     "amount": 49.99,
     "callbackUrl": "https://your-system.com/callback",
     "redirectUrl": "https://your-system.com/payment-complete"
   }
   ```
   The API will return a payment URL with `redirect-url` already included.

2. **Manually append to payment URL**:
   ```
   https://localhost:5001/payment/{paymentId}?redirect-url=https://your-system.com/payment-complete
   ```

The customer will be redirected with these query parameters:
- `payment-id` - The payment ID
- `status` - Payment status (Completed)

Example:
```
https://your-system.com/payment-complete?payment-id=order-12345&status=Completed
```

### 3. Customer Completes Payment

The customer enters credit card information on the OPA payment page.

**Payment Retry:** If a payment fails, the customer can retry with different card details. The page can be reloaded or navigated to again to retry a failed payment. Once completed successfully, the payment cannot be processed again.

### 4. Receive Callback

OPA will POST to your callback URL with:

```json
{
  "apiKey": "demo-api-key-12345",
  "paymentId": "unique-payment-id",
  "amount": 99.99,
  "status": "Completed",
  "reason": null
}
```

**Important:** Callbacks are sent for each payment attempt. If a customer retries a failed payment, you may receive multiple callbacks for the same payment ID. Your system should handle this by:
- Checking the payment status
- Implementing idempotent handling
- Using the most recent callback status

## Test Card Numbers

Use these card numbers to simulate different scenarios:

- `4111111111111111` - **Success**
- `4000000000000002` - **Card Expired**
- `4000000000000010` - **Invalid Card Number**
- `4000000000000028` - **Wrong Security Code**
- `4000000000000036` - **Insufficient Funds**

Any other 16-digit number starting with 4 will succeed. All other numbers will fail.

## Example Using curl

```bash
curl -X POST https://localhost:5001/api/payment/create \
  -H "Content-Type: application/json" \
  -H "X-API-Key: demo-api-key-12345" \
  -d '{
    "paymentId": "order-12345",
    "amount": 49.99,
    "callbackUrl": "https://webhook.site/your-unique-url",
    "redirectUrl": "https://your-system.com/payment-complete"
  }'
```