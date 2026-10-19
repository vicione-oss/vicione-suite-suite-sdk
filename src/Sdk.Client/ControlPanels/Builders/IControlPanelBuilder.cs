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
    /// <remarks>
    /// The handler is registered as <see cref="ControlPanelServiceKey{TControlPanel}">keyed service</see> and belongs to
    /// <typeparamref name="TControlPanel"/> alone: other control panels with the same <typeparamref name="TState"/> do not
    /// use it and can configure their own. A control panel that configures no save handler cannot be saved.
    /// The same applies to <see cref="WithCancelHandler{TCancelHandler}"/> and <see cref="WithResetHandler{TResetHandler}"/>,
    /// without which canceling and resetting do nothing.
    /// </remarks>
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

    /// <summary>
    /// Configures save, cancel and reset handlers that <typeparamref name="TControlPanel"/> shares with the other control panels
    /// editing its entity, e.g. a parent with its tabs, which configure them with
    /// <see cref="WithSharedHandlers(ISharedControlPanelHandlers{TState})"/> and <paramref name="sharedHandlers"/>.
    /// </summary>
    /// <remarks>
    /// Each of these control panels has an edit of its own, and the host saves, cancels and resets the running edits together.
    /// Invocations of a shared handler that run at the same time join one, so a change on several tabs neither saves the
    /// entity twice nor, for a new one, creates it twice. The handlers are registered as scoped
    /// <see cref="ControlPanelServiceKey{TControlPanel}">keyed services</see> of <typeparamref name="TControlPanel"/>, one
    /// instance shared by these control panels; share the state as well with <see cref="WithSharedState(out ISharedControlPanelState{TState})"/>,
    /// so they get the same state whichever of these control panels runs them. Control
    /// panels configuring the same handler types on their own share nothing. The handlers the control panels resolve
    /// implement <see cref="ISharedControlPanelHandler"/>. A scope edits one entity per owning control panel at a time.
    /// </remarks>
    /// <param name="configureSharedHandlers">Configures the handlers to share.</param>
    /// <param name="sharedHandlers">The handlers to pass to the other control panels editing the entity.</param>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="TControlPanel"/> has a handler of its own already for an operation it shares, which it would keep
    /// using, so it would not join the other control panels and the entity would be handled twice. Sharing the handlers of
    /// the other operations only is fine.
    /// </exception>
    /// <exception cref="NotSupportedException">The builder is not the one the SDK provides.</exception>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedHandlers(
        Action<ISharedControlPanelHandlersBuilder<TState>> configureSharedHandlers,
        out ISharedControlPanelHandlers<TState> sharedHandlers)
        => throw new NotSupportedException();

    /// <summary>
    /// Configures the handlers that another control panel shares with <typeparamref name="TControlPanel"/>, because both edit
    /// its entity, e.g. a tab with its parent.
    /// </summary>
    /// <remarks>
    /// A host resets the control panels sharing a reset handler once when they are shown, rather than once per control
    /// panel, so showing a tab does not discard the changes made on another; it tells them by
    /// <see cref="ISharedControlPanelHandler.OwnerServiceKey"/> and <see cref="ISharedControlPanelHandler.HandlerType"/>.
    /// A handler <typeparamref name="TControlPanel"/> has configured already is kept.
    /// </remarks>
    /// <exception cref="NotSupportedException">The builder is not the one the SDK provides, or <paramref name="sharedHandlers"/>
    /// were not created by it.</exception>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedHandlers(ISharedControlPanelHandlers<TState> sharedHandlers)
        => throw new NotSupportedException();

    /// <summary>
    /// Shares the state of <typeparamref name="TControlPanel"/> with the other control panels configuring
    /// <paramref name="sharedState"/> with <see cref="WithSharedState(ISharedControlPanelState{TState})"/>, e.g. a parent with
    /// its tabs, so that all of them work on one state instance per scope.
    /// </summary>
    /// <remarks>
    /// Independent of <see cref="WithSharedHandlers(Action{ISharedControlPanelHandlersBuilder{TState}}, out ISharedControlPanelHandlers{TState})"/>:
    /// control panels can share their state but keep handlers of their own, share their handlers but keep states of their
    /// own, or share both.
    /// </remarks>
    /// <param name="sharedState">The state to pass to the other control panels sharing it.</param>
    /// <exception cref="NotSupportedException">The builder is not the one the SDK provides.</exception>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedState(out ISharedControlPanelState<TState> sharedState)
        => throw new NotSupportedException();

    /// <summary>
    /// Gives <typeparamref name="TControlPanel"/> the state another control panel shares instead of a state of its own,
    /// e.g. a tab the state of its parent.
    /// </summary>
    /// <remarks>
    /// Replaces the state of <typeparamref name="TControlPanel"/>, also when called before
    /// <see cref="WithAutoDiscovery{TDescriptor}"/>. Do not give two control panels each other's state: resolving it would
    /// never end.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><paramref name="sharedState"/> is the state <typeparamref name="TControlPanel"/> shares itself.</exception>
    /// <exception cref="NotSupportedException">The builder is not the one the SDK provides, or <paramref name="sharedState"/>
    /// was not created by it.</exception>
    IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedState(ISharedControlPanelState<TState> sharedState)
        => throw new NotSupportedException();
}
