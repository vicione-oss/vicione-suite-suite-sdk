using System.Reflection;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.Internals;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// Provides static helper methods for constructing MassTransit endpoint names and addresses.
/// </summary>
public static class MessagingHelper
{
    /// <summary>
    /// The standard prefix used for instance-specific queue names.
    /// </summary>
    public const string InstanceQueueNamePrefix = "Instance";

    internal const string CommandsQueueName = "Commands";
    internal const string RequestsQueueName = "Requests";
    internal const string EventsQueueName = "Events";
    internal const string ExchangePrefix = "exchange";
    private static readonly char[] s_removeChars = ['.', '+'];

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified command type and optional instance ID.
    /// </summary>
    public static Uri GetCommandEndpointAddress(Type commandType, Guid? instanceId)
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, commandType, instanceId)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified command.
    /// </summary>
    public static Uri GetCommandEndpointAddress<TCommand>()
        where TCommand : class, ICommand
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, typeof(TCommand), null)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified instance-dependent command.
    /// </summary>
    public static Uri GetCommandEndpointAddress<TCommand>(Guid instanceId)
        where TCommand : class, IInstanceDependentCommand
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, typeof(TCommand), instanceId)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified request-response message pair.
    /// </summary>
    public static Uri GetRequestEndpointAddress<TRequest, TResponse>()
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse
        => new($"exchange:{GetRequestEndpointName(typeof(TRequest), null)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified instance-dependent request-response message pair.
    /// </summary>
    public static Uri GetRequestEndpointAddress<TRequest, TResponse>(Guid? instanceId)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse
        => new($"exchange:{GetRequestEndpointName(typeof(TRequest), instanceId)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified activity argument type.
    /// </summary>
    public static Uri GetActivityEndpointAddress<TArgument>(Guid? instanceId) where TArgument : class, IActivityArgument
        => new($"{ExchangePrefix}:{GetActivityEndpointName(typeof(TArgument), instanceId)}");

    /// <summary>
    /// Gets the MassTransit endpoint address for a specified instance-dependent activity argument type.
    /// </summary>
    public static Uri GetActivityEndpointAddress<TArgument>(Guid instanceId) where TArgument : class, IInstanceDependentActivityArgument
        => new($"{ExchangePrefix}:{GetActivityEndpointName(typeof(TArgument), instanceId)}");

    /// <summary>
    /// Gets the endpoint name for a command message.
    /// </summary>
    public static string GetCommandEndpointName(MemberInfo? consumerType, Type messageType, Guid? instanceId)
    {
        if (!messageType.HasInterface<ICommand>() && !messageType.HasInterface<IInstanceDependentCommand>())
            throw new InvalidOperationException($"A command must implement {nameof(ICommand)} or {nameof(IInstanceDependentCommand)}");

        if (!messageType.IsInstanceDependent())
            return messageType.GetEndpointName() ?? CommandsQueueName;

        if (instanceId is null)
            throw new InvalidOperationException("InstanceId must be provided, when sending to instance dependant Endpoints");

        if (consumerType?.IsReadOnlyConsumer() == false)
            throw new InvalidOperationException($"Instance dependent consumers must have the {nameof(ReadOnlyConsumerAttribute)}");

        return CleanName($"{InstanceQueueNamePrefix}_{instanceId}");
    }

    /// <summary>
    /// Gets the endpoint name for an event message.
    /// </summary>
    public static string GetEventEndpointName(MemberInfo consumerType, Type messageType, Guid? instanceId)
        => messageType.HasInterface<IInstanceDependentMessage>() || consumerType.IsReadOnlyConsumer()
            ? CleanName($"{InstanceQueueNamePrefix}_{instanceId}")
            : messageType.GetEndpointName() ?? EventsQueueName;

    /// <summary>
    /// Gets the endpoint name for a request message.
    /// </summary>
    public static string GetRequestEndpointName(Type messageType, Guid? instanceId)
        => messageType.IsInstanceDependent()
            ? CleanName($"{InstanceQueueNamePrefix}_{instanceId}")
            : messageType.GetEndpointName() ?? RequestsQueueName;

    /// <summary>
    /// Checks if a consumer type handles any instance-dependent messages.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the consumer handles at least one instance-dependent message,
    /// otherwise <see langword="true"/>.
    /// </returns>
    public static bool ConsumesInstanceDependentMessages(this Type consumerType)
        => consumerType.FindMessageTypes().Any(t => t.IsInstanceDependent());

    /// <summary>
    /// Checks if a consumer type handles request messages.
    /// </summary>
    /// <returns><see langword="true"/> if the consumer handles request messages, otherwise <see langword="false"/>.</returns>
    public static bool ConsumesRequest(this Type consumerType)
        => consumerType.FindMessageTypes().All(a => a.HasInterface(typeof(IRequest<>)));

    /// <summary>
    /// Finds all message types handled by a given consumer or activity type.
    /// </summary>
    public static IEnumerable<Type> FindMessageTypes(this Type consumingType)
    {
        foreach (var interfaceType in consumingType.GetTypeInfo().ImplementedInterfaces)
        {
            if (!interfaceType.IsGenericType)
                continue;

            var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
            if (!(genericTypeDefinition == typeof(IConsumer<>) || genericTypeDefinition == typeof(IExecuteActivity<>)))
                continue;

            var messageType = interfaceType.GetGenericArguments()[0];
            if (messageType.IsGenericType)
                continue;

            if (Equals(messageType.FullName, typeof(RoutingSlipCompleted).FullName)
                || Equals(messageType.FullName, typeof(RoutingSlipFaulted).FullName))
            {
                continue;
            }

            yield return messageType;
        }
    }

    /// <summary>
    /// Gets the custom endpoint name for a message type from its <see cref="MessageEndpointAttribute"/>
    /// </summary>
    /// <returns>The custom endpoint name if defined, otherwise <see langword="null"/>.</returns>
    public static string? GetEndpointName(this Type messageType)
        => messageType.GetAttribute<MessageEndpointAttribute>().FirstOrDefault()?.EndpointName;

    /// <summary>
    /// Checks if a message type is instance-dependent by looking for <see cref="IInstanceDependentMessage"/>.
    /// </summary>
    public static bool IsInstanceDependent(this Type messageType)
        => messageType.GetInterface<IInstanceDependentMessage>() is not null;

    /// <summary>
    /// Gets the endpoint name for an activity argument type.
    /// </summary>
    public static string GetActivityEndpointName(this Type argumentType, Guid? instanceId)
    {
        var endpointName = argumentType.GetEndpointName()
            ?? throw new InvalidOperationException("Routable activities must define an endpoint");

        if (!argumentType.IsInstanceDependent())
            return CleanName(endpointName);

        if (instanceId is null)
            throw new InvalidOperationException(
                $"{argumentType.Name} is instance-dependent but no instance id was provided when resolving its activity endpoint");

        return CleanName($"{endpointName}_{instanceId}");
    }

    /// <summary>
    /// Checks if a consumer type is marked as read-only via the <see cref="ReadOnlyConsumerAttribute"/>.
    /// </summary>
    public static bool IsReadOnlyConsumer(this MemberInfo type)
        => type.GetCustomAttribute<ReadOnlyConsumerAttribute>() is not null;

    /// <summary>
    /// Cleans a string by removing characters that are invalid in queue names.
    /// </summary>
    public static string CleanName(string name)
        => string.Concat(name.Split(s_removeChars));

    /// <summary>
    /// Creates a clean, queue-name-friendly string from a <see cref="Type"/>.
    /// </summary>
    public static string CleanName(Type type, bool fullname = false)
        => CleanName(fullname ? type.FullName! : TypeCache.GetShortName(type));
}
