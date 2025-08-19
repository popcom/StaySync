using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StaySync.Domain.Entities;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.WebApi.Security;
using Xunit;

namespace StaySync.WebApi.UnitTests.Middleware;

public class ApiKeyMiddlewareTests
{
    private static string Sha256Hex(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    [Fact]
    public async Task Missing_api_key_returns_401()
    {
        var ctx = new DefaultHttpContext();
        var repo = new Mock<IHotelRepository>();
        var current = new CurrentHotelContext();
        var mw = new ApiKeyMiddleware(_ => Task.CompletedTask);

        await mw.InvokeAsync(ctx, repo.Object, current);

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task Valid_api_key_sets_hotel_and_calls_next()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Api-Key"] = "demo-key";

        var hotel = new Hotel(Guid.NewGuid(), "Demo", "Europe/Berlin", Sha256Hex("demo-key"));
        var repo = new Mock<IHotelRepository>();
        repo.Setup(r => r.GetByApiKeyHashAsync(hotel.ApiKeyHash, It.IsAny<CancellationToken>())).ReturnsAsync(hotel);

        var current = new CurrentHotelContext();
        var called = false;
        var mw = new ApiKeyMiddleware(_ => { called = true; return Task.CompletedTask; });

        await mw.InvokeAsync(ctx, repo.Object, current);

        called.Should().BeTrue();
        current.HotelId.Should().Be(hotel.Id);
    }
}
