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
}

/// <summary>
/// Represents a registered control panel within a <see cref="IControlPanelRegistry{TClientModule}"/>.
/// </summary>
public interface IControlPanelRegistryItem<TClientModule> : IControlPanelRegistryItem
    where TClientModule : class, IClientModule;
