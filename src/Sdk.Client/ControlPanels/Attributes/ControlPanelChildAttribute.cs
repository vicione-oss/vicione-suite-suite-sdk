using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Declares that a class implementing <see cref="ControlPanelBase{TState}"/> is a child of whichever control panel
/// declares itself a parent via <see cref="ControlPanelParentAttribute{TControlPanelParentDescriptor}"/>
/// for the same <typeparamref name="TControlPanelParentDescriptor"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class ControlPanelChildAttribute<TControlPanelParentDescriptor> : Attribute, IControlPanelChildAttribute
    where TControlPanelParentDescriptor : class, IControlPanelParentDescriptor
{
    Type IControlPanelChildAttribute.ParentDescriptorType => typeof(TControlPanelParentDescriptor);
}
