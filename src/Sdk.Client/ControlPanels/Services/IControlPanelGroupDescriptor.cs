namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes a group of control panel categories.
/// </summary>
public interface IControlPanelGroupDescriptor
{
    /// <summary>
    /// Gets the group's position among all groups; lower positions render first.
    /// </summary>
    int Position { get; }
}
