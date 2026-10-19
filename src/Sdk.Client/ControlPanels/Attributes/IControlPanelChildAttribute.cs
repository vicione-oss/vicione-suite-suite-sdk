using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Attributes;

/// <summary>
/// Exposes the parent descriptor type of a <see cref="ControlPanelChildAttribute{TControlPanelParentDescriptor}"/>
/// without having to close its generic parameter.
/// </summary>
internal interface IControlPanelChildAttribute
{
    /// <summary>
    /// Gets the <see cref="IControlPanelParentDescriptor"/> type identifying the parent the control panel nests under.
    /// </summary>
    Type ParentDescriptorType { get; }
}
