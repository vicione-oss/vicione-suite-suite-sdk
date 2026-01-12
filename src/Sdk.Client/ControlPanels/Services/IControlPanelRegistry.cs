using Microsoft.AspNetCore.Authorization;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.Modules;
using Sdk.Client.Services;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Registry for control panels
/// </summary>
public interface IControlPanelRegistry : IRegistry<IControlPanelRegistryItem>;

/// <inheritdoc/>
public interface IControlPanelRegistry<TClientModule> : IControlPanelRegistry
    where TClientModule : class, IClientModule
{
    /// <summary>
    /// Registers a control panel in the registry.
    /// </summary>
    /// <param name="descriptor">Descriptor for the control panel</param>
    /// <param name="state">State for the control panel</param>
    /// <param name="categoryDescriptor">Category descriptor for the control panel</param>
    /// <param name="groupDescriptor">Optional group descriptor for the control panel, <see cref="IDefaultControlPanelGroupDescriptor"/> will be used when omitted</param>
    /// <param name="authorizationRequirement">Optional authorization requirement, otherwise <see langword="null" /> to skip authorization</param>
    IControlPanelRegistryItem Add<TComponent, TState>(IControlPanelDescriptor descriptor, TState state,
        IControlPanelCategoryDescriptor categoryDescriptor, IControlPanelGroupDescriptor? groupDescriptor = null,
        IAuthorizationRequirement? authorizationRequirement = null)
            where TComponent : ControlPanelBase<TState>
            where TState : IControlPanelState;

    /// <summary>
    /// Removes all control panels of type <typeparamref name="TControlPanel"/> from the registry.
    /// </summary>
    /// <returns>Number of control panels removed</returns>
    int Remove<TControlPanel>()
        where TControlPanel : IControlPanel;
}
