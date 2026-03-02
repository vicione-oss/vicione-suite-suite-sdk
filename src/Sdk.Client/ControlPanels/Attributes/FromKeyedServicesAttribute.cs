using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Indicates that the parameter should be bound using the keyed service registered with the <see cref="ControlPanelServiceKey{TControlPanel}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[ExcludeFromCodeCoverage]
public sealed class FromKeyedServicesAttribute<TControlPanel> : FromKeyedServicesAttribute
    where TControlPanel : ComponentBase, IControlPanel
{
    /// <summary>
    /// Creates a new <see cref="FromKeyedServicesAttribute{TControlPanel}"/> instance.
    /// </summary>
    public FromKeyedServicesAttribute()
        : base(typeof(ControlPanelServiceKey<TControlPanel>))
    { }
}
