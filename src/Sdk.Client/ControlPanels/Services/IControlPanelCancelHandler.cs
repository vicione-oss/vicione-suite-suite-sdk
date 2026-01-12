namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Control panel cancel handler
/// </summary>
public interface IControlPanelCancelHandler<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Cancel logic based on the given <paramref name="state"/> invoked when changes of the control panel associated with the given
    /// <paramref name="state"/> needs to be canceled.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the cancel operation.
    /// </remarks>
    /// <param name="state">Control panel state</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task Cancel(TState state, CancellationToken cancellationToken);
}
