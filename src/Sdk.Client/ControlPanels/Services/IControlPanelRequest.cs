using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Components;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Request for a control panel
/// </summary>
public interface IControlPanelRequest
{
    /// <summary>
    /// Raised when <see cref="Send(IControlPanelRegistryItem,Action)"/> was called
    /// </summary>
    event Func<ControlPanelRequestedEventArgs, Task>? ControlPanelRequested;

    /// <summary>
    /// Raises event <see cref="ControlPanelRequested"/> to notify about the request to display
    /// the control panel associated with the given <paramref name="controlPanelRegistryItem"/>
    /// </summary>
    /// <param name="controlPanelRegistryItem"></param>
    /// <param name="configureState">Optional action used to configure the control panel state</param>
    /// <returns>
    /// True if the request was canceled via <see cref="ControlPanelRequestedEventArgs.Cancel"/>, otherwise false.
    /// </returns>
    Task<bool> Send(IControlPanelRegistryItem controlPanelRegistryItem, Action? configureState = null);

    /// <summary>
    /// Raises event <see cref="ControlPanelRequested"/> to notify about the request to display
    /// the control panel implemented by <typeparamref name="TComponent"/>.
    /// </summary>
    /// <returns>
    /// True if the request was canceled via <see cref="ControlPanelRequestedEventArgs.Cancel"/>, otherwise false.
    /// </returns>
    Task<bool> Send<TComponent>() where TComponent : ComponentBase, IControlPanel;

    /// <summary>
    /// Raises event <see cref="ControlPanelRequested"/> to notify about the request to display
    /// the control panel implemented by <typeparamref name="TComponent"/>.
    /// </summary>
    /// <param name="configureState">Action used to configure the control panel state</param>
    /// <returns>
    /// True if the request was canceled via <see cref="ControlPanelRequestedEventArgs.Cancel"/>, otherwise false.
    /// </returns>
    Task<bool> Send<TComponent, TState>(Action<TState>? configureState)
        where TComponent : ControlPanelBase<TState>, IControlPanel
        where TState : IControlPanelState;
}
