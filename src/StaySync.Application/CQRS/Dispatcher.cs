using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StaySync.Application.Interfaces;

namespace StaySync.Application.CQRS;

public sealed class Dispatcher(IServiceProvider services) : IDispatcher
{
    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
    {
        await Validate(command, ct);
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = services.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, ct);
    }

    public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
    {
        await Validate(query, ct);
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = services.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, ct);
    }

    private async Task Validate<T>(T request, CancellationToken ct)
    {
        foreach (var v in services.GetServices<IValidator<T>>())
        {
            var res = await v.ValidateAsync(request, ct);
            if (!res.IsValid) throw new ValidationException(res.Errors);
        }
    }
}
