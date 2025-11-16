using System.Text.Json.Serialization;
using DeliFHery.Logic;
using DeliFHery.Persistence;
using DeliFHery.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid request payload",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

var useInMemoryDatabase = builder.Configuration.GetValue("Database:UseInMemory", false);
if (useInMemoryDatabase)
{
    builder.Services.AddDbContext<DeliFHeryDbContext>(options =>
    {
        options.UseInMemoryDatabase("DeliFHery");
    });
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DeliFHeryDb");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("A database connection string must be configured.");
    }

    builder.Services.AddDbContext<DeliFHeryDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });
}

builder.Services.AddDeliFHeryLogic();

var keycloakSection = builder.Configuration.GetSection(KeycloakOptions.SectionName);
builder.Services.Configure<KeycloakOptions>(keycloakSection);
var keycloakOptions = keycloakSection.Get<KeycloakOptions>() ?? throw new InvalidOperationException("Keycloak configuration missing");
if (string.IsNullOrWhiteSpace(keycloakOptions.Authority) || string.IsNullOrWhiteSpace(keycloakOptions.Audience))
{
    throw new InvalidOperationException("Keycloak authority and audience must be configured.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakOptions.Authority;
        options.Audience = keycloakOptions.Audience;
        options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.ApiUser, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            if (string.IsNullOrWhiteSpace(keycloakOptions.RequiredScope))
            {
                return true;
            }

            return context.User.HasScope(keycloakOptions.RequiredScope!);
        });
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DeliFHeryDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }
}

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var statusCode = exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status400BadRequest
                ? "Invalid request"
                : "An unexpected error occurred",
            Detail = exception?.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
