using System.Text.Json.Serialization;
using OrderManagement.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigureServices(builder.Services,  builder.Configuration, builder.Environment);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

ConfigureMiddleware(app, app.Environment);
ConfigureEndpoints(app);

app.Run();
return;

void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
{
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddXmlDataContractSerializerFormatters();
    services.AddScoped<IOrderManagementLogic,  OrderManagementLogic>();
}

void ConfigureMiddleware(IApplicationBuilder appBuilder, IHostEnvironment env)
{
    appBuilder.UseHttpsRedirection();
}

void ConfigureEndpoints(IEndpointRouteBuilder webApplication)
{
    webApplication.MapControllers();
}