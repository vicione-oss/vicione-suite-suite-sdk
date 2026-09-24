using System.Globalization;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for MassTransit <see cref="RoutingSlipCompleted"/> and <see cref="RoutingSlipFaulted"/> events.
/// </summary>
public static class RoutingSlipExtensions
{
    /// <inheritdoc cref="GetVariable{T}(IDictionary{string, object}, string)"/>
    public static T GetVariable<T>(this RoutingSlipCompleted routingSlip, string key)
        => routingSlip.Variables.GetVariable<T>(key);

    /// <inheritdoc cref="GetVariable{T}(IDictionary{string, object}, string)"/>
    public static T GetVariable<T>(this RoutingSlipFaulted routingSlip, string key)
        => routingSlip.Variables.GetVariable<T>(key);

    /// <summary>
    /// Returns variable <paramref name="key"/> converted to <typeparamref name="T"/> with the invariant culture; a
    /// <see cref="Guid"/> is parsed from its string form, which is how routing-slip variables carry it.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if the variable does not exist.</exception>
    public static T GetVariable<T>(this IDictionary<string, object> dictionary, string key)
    {
        var value = dictionary[key];

        return typeof(T) == typeof(Guid)
            ? (T)Convert.ChangeType(Guid.Parse(value.ToString()!), typeof(T), CultureInfo.InvariantCulture)
            : (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }
}
