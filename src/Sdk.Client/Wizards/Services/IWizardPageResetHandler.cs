namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Resets a wizard page whose state is <typeparamref name="TState"/>.
/// </summary>
public interface IWizardPageResetHandler<TState>
    where TState : IWizardPageState
{
    /// <summary>
    /// Called when the wizard page owning <paramref name="state"/> is reset.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the reset operation.
    /// </remarks>
    Task Reset(TState state, CancellationToken cancellationToken);
}
