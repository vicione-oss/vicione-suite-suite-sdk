using Sdk.Messaging;

namespace Sdk.Client.Infrastructure;

public interface IUiMediator
{
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
    Task Send<TCommand>(TCommand command, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand;

    Task<TResponse> Request<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse;
    Task<TResponse> Request<TRequest, TResponse>(TRequest request, Guid instanceId, CancellationToken cancellationToken = default)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse;

    IDisposable Register<TEvent>(IEventConsumer<TEvent> handler)
        where TEvent : class, IEvent;
}
