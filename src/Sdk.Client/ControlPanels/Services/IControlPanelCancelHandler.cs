namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Reverts the changes of a control panel whose state is <typeparamref name="TState"/> when they are canceled.
/// </summary>
public interface IControlPanelCancelHandler<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Called when the changes of the control panel owning <paramref name="state"/> are canceled.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the cancel operation.
    /// </remarks>
    Task Cancel(TState state, CancellationToken cancellationToken);
}
