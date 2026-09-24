using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// The result of <see cref="IControlPanelSaveHandler{TState}.Save(TState, CancellationToken)"/>: a <see cref="SaveSuccessResult"/>
/// or a <see cref="SaveErrorResult"/>.
/// </summary>
public interface ISaveResult
{
    /// <summary>
    /// Gets the message describing the outcome; optional on success.
    /// </summary>
    string? Message { get; }
}
