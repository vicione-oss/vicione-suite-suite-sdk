using System.Globalization;
using MassTransit.Courier.Contracts;

namespace Sdk.Backend.Extensions;

public static class RoutingSlipExtensions
{
    public static T GetVariable<T>(this RoutingSlipCompleted routingSlip, string key)
    {
        return routingSlip.Variables.GetVariable<T>(key);
    }

    public static T GetVariable<T>(this RoutingSlipFaulted routingSlip, string key)
    {
        return routingSlip.Variables.GetVariable<T>(key);
    }

    public static T GetVariable<T>(this IDictionary<string, object> dictionary, string key)
    {
        var value = dictionary[key];

        return typeof(T) == typeof(Guid)
            ? (T)Convert.ChangeType(Guid.Parse(value.ToString()!), typeof(T), CultureInfo.InvariantCulture)
            : (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }
}
