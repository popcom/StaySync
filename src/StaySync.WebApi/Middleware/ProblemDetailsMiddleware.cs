using FluentValidation;
using StaySync.Domain.Exceptions;

namespace StaySync.WebApi.Middleware;

public sealed class ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException v)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new
            {
                type = "https://httpstatuses.com/400",
                title = "Validation failed",
                status = 400,
                errors = v.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage))
            });
        }
        catch (NotFoundException nf)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(new { type = "https://httpstatuses.com/404", title = nf.Message, status = 404 });
        }
        catch (ConflictException cf)
        {
            ctx.Response.StatusCode = StatusCodes.Status409Conflict;
            await ctx.Response.WriteAsJsonAsync(new { type = "https://httpstatuses.com/409", title = cf.Message, status = 409 });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new { type = "https://httpstatuses.com/500", title = "Server error", status = 500 });
        }
    }
}
