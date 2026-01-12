namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes a control panel group
/// </summary>
public interface IControlPanelGroupDescriptor
{
    /// <summary>
    /// Position in the list of all groups
    /// </summary>
    /// <remarks>
    /// This property affects the render order.
    /// When Position X of Group A is lower than Position Y of Group B then Group A is rendered first.
    /// In a vertical representation this would mean that Group A is displayed above Group B.
    /// </remarks>
    int Position { get; }
}
