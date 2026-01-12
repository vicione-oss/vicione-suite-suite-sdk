using Sdk.Client.Wizards.Services;

namespace Sdk.Client.Wizards.Models;

/// <summary>
/// Return value of <see cref="IWizardPageSaveHandler{TState}.Save(TState, CancellationToken)"/>
/// </summary>
public interface ISaveResult
{
    /// <summary>
    /// Message describing the result of the associated save operation
    /// </summary>
    string? Message { get; }
}
