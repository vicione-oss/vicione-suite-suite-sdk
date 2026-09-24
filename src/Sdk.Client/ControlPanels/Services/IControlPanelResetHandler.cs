namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Resets a control panel whose state is <typeparamref name="TState"/>.
/// </summary>
public interface IControlPanelResetHandler<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Called when the control panel owning <paramref name="state"/> is reset.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the reset operation.
    /// </remarks>
    Task Reset(TState state, CancellationToken cancellationToken);
}
