using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Describes how a control panel is presented in navigation.
/// </summary>
public interface IControlPanelDescriptor
{
    /// <summary>
    /// Gets the control panel's title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the URL of the control panel's icon; <see langword="null"/> for none.
    /// </summary>
    Uri? IconUrl { get; }

    /// <summary>
    /// Gets the control panel's position within its <see cref="IControlPanelCategoryDescriptor">category</see>; lower positions
    /// render first. Control panels without a position follow, ordered by <see cref="Title"/>. Defaults to <see langword="null"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    int? Position => null;

    /// <summary>
    /// Gets whether the control panel is listed in navigation. Defaults to <see langword="true"/>.
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
