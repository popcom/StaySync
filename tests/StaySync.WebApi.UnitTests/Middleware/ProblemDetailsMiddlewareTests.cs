using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaySync.Domain.Exceptions;
using StaySync.WebApi.Middleware;
using System.Text.Json;
using Xunit;

namespace StaySync.WebApi.UnitTests.Middleware;

public class ProblemDetailsMiddlewareTests
{
    [Fact]
    public async Task NotFound_maps_to_404()
    {
        var ctx = new DefaultHttpContext();
        var mw = new ProblemDetailsMiddleware(_ => throw new NotFoundException("x"), NullLogger());

        await mw.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Conflict_maps_to_409()
    {
        var ctx = new DefaultHttpContext();
        var mw = new ProblemDetailsMiddleware(_ => throw new ConflictException("x"), NullLogger());

        await mw.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task Unknown_maps_to_500()
    {
        var ctx = new DefaultHttpContext();
        var mw = new ProblemDetailsMiddleware(_ => throw new Exception("boom"), NullLogger());

        await mw.InvokeAsync(ctx);

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    private static ILogger<ProblemDetailsMiddleware> NullLogger()
        => new Microsoft.Extensions.Logging.Abstractions.NullLogger<ProblemDetailsMiddleware>();
}
