using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Declares that a class implementing <see cref="ControlPanelBase{TState}"/> is a parent identified by
/// <typeparamref name="TControlPanelParentDescriptor"/>, so other control panels can nest under it by declaring
/// <see cref="ControlPanelChildAttribute{TControlPanelParentDescriptor}"/> for the same
/// <typeparamref name="TControlPanelParentDescriptor"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class ControlPanelParentAttribute<TControlPanelParentDescriptor> : Attribute, IControlPanelParentAttribute
    where TControlPanelParentDescriptor : class, IControlPanelParentDescriptor
{
    Type IControlPanelParentAttribute.ParentDescriptorType => typeof(TControlPanelParentDescriptor);
}
