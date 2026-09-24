using MassTransit;
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
        /// Makes the <see cref="ISuiteMediator"/> substitute answer <paramref name="request"/> with <paramref name="response"/>.
        /// </summary>
        /// <param name="request">The request to match by equality; <see langword="null"/> matches any request of the type.</param>
        /// <param name="response">The response to return.</param>
        public void SetupRequest<TRequest, TResponse>(TRequest? request, TResponse response)
            where TRequest : class, IRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), Arg.Any<CancellationToken>())
                .Returns(response);

        /// <summary>
        /// Makes the <see cref="ISuiteMediator"/> substitute fail <paramref name="request"/> with a <see cref="RequestFaultException"/>.
        /// </summary>
        public void SetupRequestFault<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<TResponse>(new RequestFaultException()));

        /// <summary>
        /// Makes the <see cref="ISuiteMediator"/> substitute answer an instance-dependent <paramref name="request"/> to
        /// <paramref name="instanceId"/> with <paramref name="response"/>.
        /// </summary>
        /// <param name="request">The request to match by equality; <see langword="null"/> matches any request of the type.</param>
        /// <param name="response">The response to return.</param>
        /// <param name="instanceId">The target instance the request must be sent to.</param>
        public void SetupRequest<TRequest, TResponse>(TRequest? request, TResponse response,
            Guid instanceId)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request ?? Arg.Any<TRequest>(), instanceId, Arg.Any<CancellationToken>())
                .Returns(response);

        /// <summary>
        /// Makes the <see cref="ISuiteMediator"/> substitute fail an instance-dependent <paramref name="request"/> to
        /// <paramref name="instanceId"/> with a <see cref="RequestFaultException"/>.
        /// </summary>
        public void SetupRequestFault<TRequest, TResponse>(TRequest request, Guid instanceId)
            where TRequest : class, IInstanceDependentRequest<TResponse>
            where TResponse : class, IResponse
            => mediator.Request<TRequest, TResponse>(request, instanceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<TResponse>(new RequestFaultException()));
    }
}
