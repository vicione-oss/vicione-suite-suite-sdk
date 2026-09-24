using Sdk.Authorization;
using Sdk.Client.ControlPanels.Attributes;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Builders;

/// <summary>
/// Configures how a control panel is discovered and which handlers save, cancel and reset it.
/// </summary>
public interface IControlPanelBuilder<TClientModule, TControlPanel, TState>
    where TClientModule : class, IClientModule
    where TControlPanel : class, IControlPanel
    where TState : IControlPanelState
{
    /// <summary>
    /// Configures auto-discovery which adds <typeparamref name="TControlPanel"/> to
    /// <see cref="IControlPanelRegistry{TClientModule}"/> with the given <typeparamref name="TDescriptor"/>
    /// and <typeparamref name="TState"/>. Optional aspects like category, group and
    /// authorization requirement are retrieved from attributes.
    /// </summary>
    /// <remarks>
    /// The method will register <see cref="IControlPanelDescriptor{TControlPanel}"/> as scoped service and
    /// <typeparamref name="TState"/> as <see cref="ControlPanelServiceKey{TControlPanel}">keyed service</see>.
    /// The implementation of <see cref="IControlPanelCategoryDescriptor"/> is retrieved from
    /// <see cref="ControlPanelCategoryAttribute{TControlPanelCategoryDescriptor}"/> and registered as scoped service;
    /// a control panel without that attribute is not added to the registry.
    /// The implementation of <see cref="IControlPanelGroupDescriptor"/> is retrieved from
    /// <see cref="ControlPanelGroupAttribute{TControlPanelGroupDescriptor}"/> and registered as scoped service.
    /// Authorization information is retrieved from <see cref="ModuleAuthorizeAttribute"/>.
    /// </remarks>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithAutoDiscovery<TDescriptor>()
        where TDescriptor : class, IControlPanelDescriptor<TControlPanel>;

    /// <summary>
    /// Configures <typeparamref name="TSaveHandler"/> for handling save requests in context of <typeparamref name="TControlPanel"/>.
    /// </summary>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IControlPanelSaveHandler<TState>;

    /// <summary>
    /// Configures <typeparamref name="TCancelHandler"/> for handling cancel requests in context of <typeparamref name="TControlPanel"/>.
    /// </summary>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IControlPanelCancelHandler<TState>;

    /// <summary>
    /// Configures <typeparamref name="TResetHandler"/> for handling reset requests in context of <typeparamref name="TControlPanel"/>.
    /// </summary>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IControlPanelResetHandler<TState>;
}
