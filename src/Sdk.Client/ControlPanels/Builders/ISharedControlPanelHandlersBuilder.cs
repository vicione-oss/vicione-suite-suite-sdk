using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <summary>
/// Configures the handlers a control panel shares with the other control panels editing its entity, see
/// <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}.WithSharedHandlers(Action{ISharedControlPanelHandlersBuilder{TState}}, out ISharedControlPanelHandlers{TState})"/>.
/// </summary>
public interface ISharedControlPanelHandlersBuilder<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Shares <typeparamref name="TSaveHandler"/>, which saves the whole entity.
    /// </summary>
    ISharedControlPanelHandlersBuilder<TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IControlPanelSaveHandler<TState>;

    /// <summary>
    /// Shares <typeparamref name="TCancelHandler"/>, which reverts the changes to the whole entity.
    /// </summary>
    ISharedControlPanelHandlersBuilder<TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IControlPanelCancelHandler<TState>;

    /// <summary>
    /// Shares <typeparamref name="TResetHandler"/>, which loads the whole entity.
    /// </summary>
    /// <remarks>
    /// Share it with every control panel editing the entity, so canceling a change made on any of them resets the entity.
    /// </remarks>
    ISharedControlPanelHandlersBuilder<TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IControlPanelResetHandler<TState>;
}
