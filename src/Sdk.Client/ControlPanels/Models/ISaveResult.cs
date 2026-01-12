using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// Return value of <see cref="IControlPanelSaveHandler{TState}.Save(TState, CancellationToken)"/>
/// </summary>
public interface ISaveResult
{
    /// <summary>
    /// Message describing the result of the associated save operation
    /// </summary>
    string? Message { get; }
}
