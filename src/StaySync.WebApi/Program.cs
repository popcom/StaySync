using Microsoft.OpenApi.Models;
using Serilog;
using StaySync.Application.DependencyInjection;
using StaySync.Application.Interfaces;
using StaySync.Infrastructure.DependencyInjection;
using StaySync.Infrastructure.Persistence;
using StaySync.WebApi.Middleware;
using StaySync.WebApi.Security;
using StaySync.WebApi.Time;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

// Connection string
var sql = builder.Configuration.GetConnectionString("Sql") ?? "";

// Application Layer
builder.Services.AddApplication();

// Infrastructure Layer
builder.Services.AddInfrastructure(sql);

// Request-scoped hotel context + clock
builder.Services.AddScoped<CurrentHotelContext>();
builder.Services.AddScoped<ICurrentHotelContext>(sp => sp.GetRequiredService<CurrentHotelContext>());
builder.Services.AddScoped<IHotelLocalClock, HotelLocalClock>();

// Controllers + DateOnly JSON converters
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new StaySync.WebApi.Serialization.DateOnlyJsonConverter());
        o.JsonSerializerOptions.Converters.Add(new StaySync.WebApi.Serialization.NullableDateOnlyJsonConverter());
    });

// Swagger with API Key
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StaySync API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Enter your API key. Example: demo-key",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Migrate + seed
await app.Services.MigrateAndSeedAsync(app.Lifetime.ApplicationStopping);

// Public Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ProblemDetails then API key
app.UseMiddleware<ProblemDetailsMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();

// This partial class is used to allow the Program class to be extended in tests, such as for WebApplicationFactory.
public partial class Program { }

