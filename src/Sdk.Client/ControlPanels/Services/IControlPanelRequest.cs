using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Asks the host to display a control panel, e.g. one hidden from navigation.
/// </summary>
public interface IControlPanelRequest
{
    /// <summary>
    /// Raised by every <c>Send</c> overload; a handler can cancel the request.
    /// </summary>
    event Func<ControlPanelRequestedEventArgs, Task>? ControlPanelRequested;

    /// <summary>
    /// Requests the control panel of <paramref name="controlPanelRegistryItem"/> by raising <see cref="ControlPanelRequested"/>.
    /// </summary>
    /// <param name="controlPanelRegistryItem">The registry item of the control panel to display.</param>
    /// <param name="configureState">Configures the state before the panel is shown; <see langword="null"/> leaves it unchanged.</param>
    /// <returns>
    /// <see langword="true"/> if a handler canceled the request via <see cref="ControlPanelRequestedEventArgs.Cancel"/>.
    /// </returns>
    Task<bool> Send(IControlPanelRegistryItem controlPanelRegistryItem, Action? configureState = null);

    /// <summary>
    /// Requests the control panel implemented by <typeparamref name="TComponent"/> by raising <see cref="ControlPanelRequested"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a handler canceled the request via <see cref="ControlPanelRequestedEventArgs.Cancel"/>.
    /// </returns>
    Task<bool> Send<TComponent>() where TComponent : ComponentBase, IControlPanel;

    /// <summary>
    /// Requests the control panel implemented by <typeparamref name="TComponent"/> by raising <see cref="ControlPanelRequested"/>.
    /// </summary>
    /// <param name="configureState">Configures the state before the panel is shown; <see langword="null"/> leaves it unchanged.</param>
    /// <returns>
    /// <see langword="true"/> if a handler canceled the request via <see cref="ControlPanelRequestedEventArgs.Cancel"/>.
    /// </returns>
    Task<bool> Send<TComponent, TState>(Action<TState>? configureState)
        where TComponent : ControlPanelBase<TState>, IControlPanel
        where TState : IControlPanelState;
}
