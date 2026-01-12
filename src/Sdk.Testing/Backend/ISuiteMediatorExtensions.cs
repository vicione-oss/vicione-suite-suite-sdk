using MassTransit;
using NSubstitute;
using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace Sdk.Testing.Backend;

public static class ISuiteMediatorExtensions
{
    public static void SetupRequest<TRequest, TResponse>(this ISuiteMediator mediator, TRequest? request, TResponse response)
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse
        => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);

    public static void SetupRequestFault<TRequest, TResponse>(this ISuiteMediator mediator, TRequest request)
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse
    {
        mediator.Request<TRequest, TResponse>(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<TResponse>(new RequestFaultException()));
    }

    public static void SetupRequest<TRequest, TResponse>(this ISuiteMediator mediator, TRequest? request, TResponse response, Guid instanceId)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse
        => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), instanceId, Arg.Any<CancellationToken>())
            .Returns(response);

    public static void SetupRequestFault<TRequest, TResponse>(this ISuiteMediator mediator, TRequest request, Guid instanceId)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse
    {
        mediator.Request<TRequest, TResponse>(request, instanceId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<TResponse>(new RequestFaultException()));
    }
}
