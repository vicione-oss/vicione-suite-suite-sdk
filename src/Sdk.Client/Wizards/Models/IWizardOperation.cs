namespace Sdk.Client.Wizards.Models;

/// <summary>
/// Provides properties to describe a wizard operation.
/// </summary>
public interface IWizardOperation
{
    /// <summary>
    /// Gets the text the host displays while the operation runs.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the expected duration in milliseconds; the host uses it to detect an operation that takes longer than expected.
    /// </summary>
    int? EstimatedDurationMs { get; }
}
