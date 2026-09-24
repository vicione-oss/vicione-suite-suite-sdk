using Sdk.Client.ControlPanels.Models;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Saves a control panel whose state is <typeparamref name="TState"/>.
/// </summary>
public interface IControlPanelSaveHandler<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Called when the control panel owning <paramref name="state"/> is saved.
    /// </summary>
    /// <remarks>
    /// Report expected outcomes, including validation failures, through the return value. Throw for anything unexpected;
    /// an <see cref="OperationCanceledException"/> means the save was canceled.
    /// </remarks>
    /// <returns><see cref="SaveSuccessResult"/> or <see cref="SaveErrorResult"/></returns>
    Task<ISaveResult> Save(TState state, CancellationToken cancellationToken);
}
