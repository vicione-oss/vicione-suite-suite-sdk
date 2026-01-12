using System.Globalization;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for MassTransit <see cref="RoutingSlipCompleted"/> and <see cref="RoutingSlipFaulted"/> events.
/// </summary>
public static class RoutingSlipExtensions
{
    /// <summary>
    /// Gets a strongly-typed variable from the routing slip's variable collection.
    /// </summary>
    public static T GetVariable<T>(this RoutingSlipCompleted routingSlip, string key)
        => routingSlip.Variables.GetVariable<T>(key);

    /// <summary>
    /// Gets a strongly-typed variable from the routing slip's variable collection.
    /// </summary>
    public static T GetVariable<T>(this RoutingSlipFaulted routingSlip, string key)
        => routingSlip.Variables.GetVariable<T>(key);

    /// <summary>
    /// Gets a strongly-typed variable from a dictionary, with special handling for Guids.
    /// </summary>
    public static T GetVariable<T>(this IDictionary<string, object> dictionary, string key)
    {
        var value = dictionary[key];

        return typeof(T) == typeof(Guid)
            ? (T)Convert.ChangeType(Guid.Parse(value.ToString()!), typeof(T), CultureInfo.InvariantCulture)
            : (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }
}
