namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Reverts the changes of a wizard page whose state is <typeparamref name="TState"/> when they are canceled.
/// </summary>
public interface IWizardPageCancelHandler<TState>
    where TState : IWizardPageState
{
    /// <summary>
    /// Called when the changes of the wizard page owning <paramref name="state"/> are canceled.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the cancel operation.
    /// </remarks>
    Task Cancel(TState state, CancellationToken cancellationToken);
}
