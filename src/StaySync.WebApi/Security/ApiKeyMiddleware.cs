using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using StaySync.Domain.Interfaces.Repositories;

namespace StaySync.WebApi.Security;

public sealed class ApiKeyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx, IHotelRepository hotels, CurrentHotelContext current)
    {
        if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var values))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsJsonAsync(new { title = "API key required" });
            return;
        }

        var apiKey = values.ToString();
        var hash = Sha256(apiKey);

        var hotel = await hotels.GetByApiKeyHashAsync(hash, ctx.RequestAborted);
        if (hotel is null)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsJsonAsync(new { title = "Invalid API key" });
            return;
        }

        current.HotelId = hotel.Id;

        // Correlation id for logs/traces
        if (!ctx.Request.Headers.TryGetValue("X-Request-Id", out _))
            ctx.Response.Headers["X-Request-Id"] = Guid.NewGuid().ToString("N");

        await next(ctx);
    }

    private static string Sha256(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
