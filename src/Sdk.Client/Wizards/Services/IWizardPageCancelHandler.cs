namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Wizard page cancel handler
/// </summary>
/// <typeparam name="TState">Type of the wizard page state</typeparam>
public interface IWizardPageCancelHandler<TState>
    where TState : IWizardPageState
{
    /// <summary>
    /// Cancel logic based on the given <paramref name="state"/> invoked when changes of the wizard page associated with the given
    /// <paramref name="state"/> needs to be canceled.
    /// </summary>
    /// <remarks>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the cancel operation.
    /// </remarks>
    /// <param name="state">Wizard page state</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task Cancel(TState state, CancellationToken cancellationToken);
}
