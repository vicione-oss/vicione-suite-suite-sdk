using Sdk.Client.ControlPanels.Models;
using Sdk.Client.Infrastructure;
using ViciOne.Ui.Localization.Resources;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Abstract base class for control panel save handlers that implements the fire-and-correlate
/// messaging pattern via <see cref="CompletionSourceHandlerBase{TServiceResult}"/>.
/// </summary>
/// <remarks>
/// Subclasses implement <see cref="IControlPanelSaveHandler{TState}.Save"/> by constructing
/// the appropriate command and delegating to one of the protected
/// <c>SendAndWaitForCompletion</c> overloads inherited from the base class.
/// </remarks>
/// <typeparam name="TState">
/// The control panel state type passed to <see cref="IControlPanelSaveHandler{TState}.Save"/>.
/// </typeparam>
public abstract class ControlPanelSaveHandlerBase<TState>(IUiMediator uiMediator) : CompletionSourceHandlerBase<ISaveResult>(uiMediator),
    IControlPanelSaveHandler<TState>
    where TState : class, IControlPanelState
{
    /// <inheritdoc/>
    public abstract Task<ISaveResult> Save(TState state, CancellationToken cancellationToken);

    /// <inheritdoc/>
    protected override ISaveResult CreateSuccessResult()
        => new SaveSuccessResult();

    /// <inheritdoc/>
    protected override ISaveResult CreateErrorResult(string errorMessage, int? errorCode = null)
        => new SaveErrorResult(errorMessage ?? CommonPhrases.AnUnexpectedErrorOccurred, errorCode);
}
