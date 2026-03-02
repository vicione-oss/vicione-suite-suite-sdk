using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;

namespace Sdk.Client.NotificationArea.Attributes;

/// <summary>
/// Indicates that the parameter should be bound using the keyed service registered with the <see cref="NotificationElementServiceKey{TClientModule, TNotificationElement}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[ExcludeFromCodeCoverage]
public sealed class FromKeyedServicesAttribute<TClientModule, TNotificationElement> : FromKeyedServicesAttribute
    where TClientModule : class, IClientModule
    where TNotificationElement : ComponentBase, INotificationElement
{
    /// <summary>
    /// Creates a new <see cref="FromKeyedServicesAttribute{TClientModule, TNotificationElement}"/> instance.
    /// </summary>
    public FromKeyedServicesAttribute()
        : base(typeof(NotificationElementServiceKey<TClientModule, TNotificationElement>))
    { }
}
