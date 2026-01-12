namespace Sdk.Client.Wizards.Models;

/// <inheritdoc/>
public sealed class WizardOperation : IWizardOperation
{
    /// <inheritdoc/>
    public string? Description { get; init; }

    /// <inheritdoc/>
    public int? EstimatedDurationMs { get; init; }
}
