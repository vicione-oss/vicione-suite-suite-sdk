using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Exposes the parent descriptor type of a <see cref="ControlPanelParentAttribute{TControlPanelParentDescriptor}"/>
/// without having to close its generic parameter.
/// </summary>
internal interface IControlPanelParentAttribute
{
    /// <summary>
    /// Gets the <see cref="IControlPanelParentDescriptor"/> type identifying the control panel as a parent.
    /// </summary>
    Type ParentDescriptorType { get; }
}
