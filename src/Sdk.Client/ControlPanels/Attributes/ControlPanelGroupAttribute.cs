using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Links a class implementing <see cref="ControlPanelBase{TState}"/> to a group described by
/// <typeparamref name="TControlPanelGroupDescriptor"/>.
/// </summary>
/// <remarks>
/// This configuration affects control panels registered via
/// <see cref="IServiceCollectionExtensions.AddControlPanel{TClientModule, TControlPanel, TState}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
[ExcludeFromCodeCoverage]
public sealed class ControlPanelGroupAttribute<TControlPanelGroupDescriptor> : Attribute
    where TControlPanelGroupDescriptor : class, IControlPanelGroupDescriptor;
