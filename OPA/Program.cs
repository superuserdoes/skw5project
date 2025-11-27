using Microsoft.AspNetCore.Mvc;
using OPA;
using OPA.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for callbacks
builder.Services.AddHttpClient();

// Register PaymentStore as singleton
builder.Services.AddSingleton<PaymentStore>();

var app = builder.Build();

// Get the configured API key
var apiKey = app.Configuration["OPA:ApiKey"];

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// OPA Payment API Endpoint
app.MapPost("/api/payment/create", (
    [FromHeader(Name = "X-API-Key")] string requestApiKey,
    CreatePaymentRequest request,
    HttpContext httpContext,
    PaymentStore paymentStore) =>
{
    // Validate API key against configured value
    if (string.IsNullOrEmpty(requestApiKey) || requestApiKey != apiKey)
    {
        return Results.Unauthorized();
    }

    // Create payment
    var payment = new Payment
    {
        PaymentId = request.PaymentId,
        ApiKey = apiKey,
        Amount = request.Amount,
        CallbackUrl = request.CallbackUrl
    };

    // Add payment to store
    if (!paymentStore.TryAddPayment(payment))
    {
        return Results.Conflict(new { error = "Payment ID already exists", paymentId = request.PaymentId });
    }

    // Generate payment URL
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    var paymentUrl = $"{baseUrl}/payment/{payment.PaymentId}";

    // Add redirect-url query parameter if provided
    if (!string.IsNullOrEmpty(request.RedirectUrl))
    {
        var encodedRedirectUrl = Uri.EscapeDataString(request.RedirectUrl);
        paymentUrl += $"?redirect-url={encodedRedirectUrl}";
    }

    return Results.Ok(new { PaymentUrl = paymentUrl });
});

app.Run();

