using System.Reflection;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.Internals;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

public static class MessagingHelper
{
    public const string InstanceQueueNamePrefix = "Instance";

    internal const string CommandsQueueName = "Commands";
    internal const string RequestsQueueName = "Requests";
    internal const string EventsQueueName = "Events";
    internal const string ExchangePrefix = "exchange";
    private static readonly char[] s_removeChars = ['.', '+'];

    public static Uri GetCommandEndpointAddress(Type commandType, Guid? instanceId)
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, commandType, instanceId)}");

    public static Uri GetCommandEndpointAddress<TCommand>()
        where TCommand : class, ICommand
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, typeof(TCommand), null)}");

    public static Uri GetCommandEndpointAddress<TCommand>(Guid instanceId)
        where TCommand : class, IInstanceDependentCommand
        => new($"{ExchangePrefix}:{GetCommandEndpointName(null, typeof(TCommand), instanceId)}");

    public static Uri GetRequestEndpointAddress<TRequest, TResponse>()
        where TRequest : class, IRequest<TResponse>
        where TResponse : class, IResponse
        => new($"exchange:{GetRequestEndpointName(typeof(TRequest), null)}");

    public static Uri GetRequestEndpointAddress<TRequest, TResponse>(Guid? instanceId)
        where TRequest : class, IInstanceDependentRequest<TResponse>
        where TResponse : class, IResponse
        => new($"exchange:{GetRequestEndpointName(typeof(TRequest), instanceId)}");

    public static Uri GetActivityEndpointAddress<TArgument>(Guid? instanceId) where TArgument : class, IActivityArgument
        => new($"{ExchangePrefix}:{GetActivityEndpointName(typeof(TArgument), instanceId)}");

    public static string GetCommandEndpointName(MemberInfo? consumerType, Type messageType, Guid? instanceId)
    {
        if (!messageType.HasInterface<ICommand>() && !messageType.HasInterface<IInstanceDependentCommand>())
            throw new InvalidOperationException(
                $"A command must implement {nameof(ICommand)} or {nameof(IInstanceDependentCommand)}");

        // On instance dependent commands the MessageEndpointAttribute has no effect
        // maybe that is something to reconsider in the future
        if (!messageType.IsInstanceDependent())
            return messageType.GetEndpointName() ?? CommandsQueueName;

        if (instanceId is null)
            throw new InvalidOperationException(
                "InstanceId must be provided, when sending to instance dependant Endpoints");

        // We do not know the consumer when sending, so there is no way to ensure it is readonly.
        // However, during initialization the consumer-message-pair is already validated, so we can ignore that
        if (consumerType?.IsReadOnlyConsumer() == false)
            throw new InvalidOperationException($"Instance dependent consumers must have the {nameof(ReadOnlyConsumerAttribute)}");

        return CleanName($"{InstanceQueueNamePrefix}_{instanceId}");
    }

    public static string GetEventEndpointName(MemberInfo consumerType, Type messageType, Guid? instanceId)
        => messageType.HasInterface<IInstanceDependentMessage>() || consumerType.IsReadOnlyConsumer()
            ? CleanName($"{InstanceQueueNamePrefix}_{instanceId}")
            : messageType.GetEndpointName() ?? EventsQueueName;

    public static string GetRequestEndpointName(Type messageType, Guid? instanceId)
        => messageType.IsInstanceDependent()
            ? CleanName($"{InstanceQueueNamePrefix}_{instanceId}")
            : messageType.GetEndpointName() ?? RequestsQueueName;

    public static bool ConsumesInstanceDependentMessages(this Type consumerType) => consumerType.FindMessageTypes().Any(t => t.IsInstanceDependent());

    public static bool ConsumesRequest(this Type consumerType) => consumerType.FindMessageTypes().All(a => a.HasInterface(typeof(IRequest<>)));

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

            // here we have for instance RoutingSlipCompleted|Faulted + Message + Fault<Message>
            if (Equals(messageType.FullName, typeof(RoutingSlipCompleted).FullName)
                || Equals(messageType.FullName, typeof(RoutingSlipFaulted).FullName))
                continue;

            yield return messageType;
        }
    }

    public static string? GetEndpointName(this Type messageType)
        => messageType.GetAttribute<MessageEndpointAttribute>().FirstOrDefault()?.EndpointName;

    public static bool IsInstanceDependent(this Type messageType)
        => messageType.GetInterface<IInstanceDependentMessage>() is not null;

    public static string GetActivityEndpointName(this Type argumentType, Guid? instanceId)
    {
        var endpointName = argumentType.GetEndpointName()
            ?? throw new InvalidOperationException("Routable activities must define an endpoint");

        return argumentType.IsInstanceDependent()
            ? CleanName($"{endpointName}_{instanceId}")
            : CleanName(endpointName);
    }

    public static bool IsReadOnlyConsumer(this MemberInfo type)
        => type.GetCustomAttribute<ReadOnlyConsumerAttribute>() is not null;

    public static string CleanName(string name) => string.Join("", name.Split(s_removeChars));

    public static string CleanName(Type type, bool fullname = false)
        => CleanName(fullname ? type.FullName! : TypeCache.GetShortName(type));
}
