using MassTransit;
using NSubstitute;
using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="ISuiteMediator"/> to simplify mocking of mediator behavior for testing.
/// </summary>
public static class ISuiteMediatorExtensions
{
    extension(ISuiteMediator mediator)
    {
        /// <summary>
        /// Sets up a mocked <see cref="ISuiteMediator"/> to return a specific response for a given request.
        /// </summary>
        public void SetupRequest<TRequest, TResponse>(TRequest? request, TResponse response)
            where TRequest : class, IRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), Arg.Any<CancellationToken>())
                .Returns(response);

        /// <summary>
        /// Sets up a mocked <see cref="ISuiteMediator"/> to return a <see cref="RequestFaultException"/> for a given request.
        /// </summary>
        public void SetupRequestFault<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<TResponse>(new RequestFaultException()));

        /// <summary>
        /// Sets up a mocked <see cref="ISuiteMediator"/> to return a specific response for a given instance-dependent request.
        /// </summary>
        public void SetupRequest<TRequest, TResponse>(TRequest? request, TResponse response,
            Guid instanceId)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), instanceId, Arg.Any<CancellationToken>())
                .Returns(response);

        /// <summary>
        /// Sets up a mocked <see cref="ISuiteMediator"/> to return a <see cref="RequestFaultException"/>
        /// for a given instance-dependent request.
        /// </summary>
        public void SetupRequestFault<TRequest, TResponse>(TRequest request, Guid instanceId)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request, instanceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<TResponse>(new RequestFaultException()));
    }
}
