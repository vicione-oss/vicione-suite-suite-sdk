using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Item to register a control panel in a registry
/// </summary>
public interface IControlPanelRegistryItem
{
    Type ComponentType { get; }
    IControlPanelDescriptor Descriptor { get; }
    IControlPanelCategoryDescriptor CategoryDescriptor { get; }
    IControlPanelGroupDescriptor GroupDescriptor { get; }
    IControlPanelState State { get; }

    /// <summary>
    /// Optional authorization requirement, otherwise <see langword="null" /> to skip authorization
    /// </summary>
    IAuthorizationRequirement? AuthorizationRequirement { get; }
}

/// <summary>
/// Item to register a control panel in a registry for a client module
/// </summary>
public interface IControlPanelRegistryItem<TClientModule> : IControlPanelRegistryItem
    where TClientModule : class, IClientModule;
