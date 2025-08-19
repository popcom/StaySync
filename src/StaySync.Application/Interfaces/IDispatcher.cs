namespace StaySync.Application.Interfaces;

public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
    Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
}
