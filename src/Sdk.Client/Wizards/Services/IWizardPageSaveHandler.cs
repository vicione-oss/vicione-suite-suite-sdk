using Sdk.Client.Wizards.Models;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Saves a wizard page whose state is <typeparamref name="TState"/>.
/// </summary>
public interface IWizardPageSaveHandler<TState>
    where TState : IWizardPageState
{
    /// <summary>
    /// Called when the wizard page owning <paramref name="state"/> is saved.
    /// </summary>
    /// <remarks>
    /// Report expected outcomes, including validation failures, through the return value. Throw for anything unexpected;
    /// an <see cref="OperationCanceledException"/> means the save was canceled.
    /// </remarks>
    /// <returns><see cref="SaveSuccessResult"/> or <see cref="SaveErrorResult"/></returns>
    Task<ISaveResult> Save(TState state, CancellationToken cancellationToken);
}
