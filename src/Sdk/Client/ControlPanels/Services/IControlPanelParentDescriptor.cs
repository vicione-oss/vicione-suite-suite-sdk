namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Marker interface identifying a control panel as a possible parent for other control panels.
/// </summary>
/// <remarks>
/// <para>
/// Implementations act as a shared linking key between a parent control panel, self-declared via
/// <c>Sdk.Client.ControlPanels.Attributes.ControlPanelParentAttribute{TControlPanelParentDescriptor}</c>, and any
/// number of child control panels referencing it via
/// <c>Sdk.Client.ControlPanels.Attributes.ControlPanelChildAttribute{TControlPanelParentDescriptor}</c>.
/// </para>
/// <para>
/// The parent's own <c>Sdk.Client.ControlPanels.Services.IControlPanelDescriptor</c> implementation is usually the best linking key: let it
/// additionally implement this interface and use it as the type argument on both sides, so that a child reads
/// unambiguously as "a child of the control panel described by this descriptor".
/// <c>Sdk.Client.ControlPanels.Services.IControlPanelDescriptor</c> deliberately does not inherit this interface, so a control panel is only usable
/// as a parent when its descriptor opts in.
/// </para>
/// <para>
/// Use a dedicated class implementing only this interface when no suitable descriptor exists, for example when several
/// control panels share one descriptor class. The link is matched by type, so a shared type cannot identify a single parent.
/// </para>
/// </remarks>
public interface IControlPanelParentDescriptor;
