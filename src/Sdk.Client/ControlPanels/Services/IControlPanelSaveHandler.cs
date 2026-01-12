using Sdk.Client.ControlPanels.Models;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Control panel save handler
/// </summary>
/// <typeparam name="TState">Type of the control panel state</typeparam>
public interface IControlPanelSaveHandler<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Save logic based on the given <paramref name="state"/> invoked when the control panel associated with the given <paramref name="state"/>
    /// needs to be saved.
    /// </summary>
    /// <remarks>
    /// Expected outcomes of the save operation should be indicated by the return value.
    ///
    /// <para>
    /// Unexpected behavior should be indicated by throwing an exception of any kind except <see cref="OperationCanceledException"/>,
    /// which indicates cancellation of the save operation.
    /// </para>
    /// </remarks>
    /// <returns><see cref="SaveSuccessResult"/> or <see cref="SaveErrorResult"/></returns>
    Task<ISaveResult> Save(TState state, CancellationToken cancellationToken);
}
