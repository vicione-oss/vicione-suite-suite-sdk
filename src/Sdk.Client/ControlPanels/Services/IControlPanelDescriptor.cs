using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes common characteristics of a control panel
/// </summary>
public interface IControlPanelDescriptor
{
    /// <summary>
    /// Title of the settings provided by the control panel
    /// </summary>
    string Title { get; }

    /// <summary>
    /// URL to the icon representing the settings provided by the control panel
    /// </summary>
    Uri? IconUrl { get; }

    /// <summary>
    /// Optional position in the list of all control panels of a <see cref="IControlPanelCategoryDescriptor">category</see>
    /// </summary>
    /// <remarks>
    /// <para>This property affects the render order.</para>
    /// <para>
    /// When Position X of control panel A is lower than Position Y of control panel B then control panel A is rendered first.
    /// In a vertical representation this would mean that control panel A is displayed above control panel B.
    /// </para>
    /// <para>If not set then the control panel is rendered after all control panels having a position in alphabetic order using <see cref="Title"/>.</para>
    /// </remarks>
    [ExcludeFromCodeCoverage]
    int? Position => null;

    /// <summary>
    /// True when the control panel should be displayed in navigation, otherwise false.
    /// </summary>
    /// <remarks>
    /// Hidden control panels can be shown dynamically from code via <see cref="IControlPanelRequest"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    bool ShowInNavigation => true;
}

/// <summary>
/// Describes common characteristics of a control panel of the given type.
/// </summary>
/// <remarks>
/// This interface is used in connection with DI to implement / inject a descriptor based on a given control panel type.
/// </remarks>
/// <typeparam name="TControlPanel">Type of the control panel component</typeparam>
public interface IControlPanelDescriptor<TControlPanel> : IControlPanelDescriptor
    where TControlPanel : class, IControlPanel;
