using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Represents a registered control panel within a <see cref="IControlPanelRegistry"/>.
/// </summary>
public interface IControlPanelRegistryItem
{
    /// <summary>
    /// Gets the type of the Blazor component that renders the control panel.
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Gets the primary descriptor containing the title and icon for the control panel.
    /// </summary>
    IControlPanelDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the descriptor for the category this control panel belongs to.
    /// </summary>
    IControlPanelCategoryDescriptor CategoryDescriptor { get; }

    /// <summary>
    /// Gets the descriptor for the group this control panel belongs to.
    /// </summary>
    IControlPanelGroupDescriptor GroupDescriptor { get; }

    /// <summary>
    /// Gets the state object that holds the dynamic data for the control panel.
    /// </summary>
    IControlPanelState State { get; }

    /// <summary>
    /// Gets the optional authorization requirement that must be met for the control panel to be accessible.
    /// </summary>
    IAuthorizationRequirement? AuthorizationRequirement { get; }

    /// <summary>
    /// Gets the descriptor of the parent control panel this control panel nests under, otherwise
    /// <see langword="null"/> when this control panel is a root, top-level control panel.
    /// </summary>
    /// <remarks>
    /// The registry stores this descriptor as given and never resolves it, so a control panel can be registered
    /// before the parent it names. It is the <see cref="AsParent"/> of the parent control panel.
    /// </remarks>
    IControlPanelParentDescriptor? ChildOf => null;

    /// <summary>
    /// Gets the descriptor other control panels reference to nest under this control panel, otherwise
    /// <see langword="null"/> when it cannot act as a parent.
    /// </summary>
    IControlPanelParentDescriptor? AsParent => null;
}

/// <summary>
/// Represents a registered control panel within a <see cref="IControlPanelRegistry{TClientModule}"/>.
/// </summary>
public interface IControlPanelRegistryItem<TClientModule> : IControlPanelRegistryItem
    where TClientModule : class, IClientModule;
