using Microsoft.AspNetCore.Authorization;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.Modules;
using Sdk.Client.Services;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Holds a module's control panels; this non-generic view lets the host enumerate every module's registry.
/// </summary>
public interface IControlPanelRegistry : IRegistry<IControlPanelRegistryItem>;

/// <inheritdoc/>
public interface IControlPanelRegistry<TClientModule> : IControlPanelRegistry
    where TClientModule : class, IClientModule
{
    /// <summary>
    /// Registers a control panel in the registry.
    /// </summary>
    /// <param name="descriptor">Describes how the control panel is presented.</param>
    /// <param name="state">The control panel's state.</param>
    /// <param name="categoryDescriptor">The category the control panel is listed under.</param>
    /// <param name="groupDescriptor">The group; <see langword="null"/> means <see cref="IDefaultControlPanelGroupDescriptor"/>.</param>
    /// <param name="authorizationRequirement">The access requirement; <see langword="null"/> skips authorization.</param>
    IControlPanelRegistryItem Add<TComponent, TState>(IControlPanelDescriptor descriptor, TState state,
        IControlPanelCategoryDescriptor categoryDescriptor, IControlPanelGroupDescriptor? groupDescriptor = null,
        IAuthorizationRequirement? authorizationRequirement = null)
            where TComponent : ControlPanelBase<TState>
            where TState : IControlPanelState;

    /// <summary>
    /// Removes all control panels of type <typeparamref name="TControlPanel"/> from the registry.
    /// </summary>
    /// <returns>The number of control panels removed.</returns>
    int Remove<TControlPanel>()
        where TControlPanel : IControlPanel;
}
