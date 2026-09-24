namespace Sdk.Client.Wizards.Models;

/// <summary>
/// The result of a successful save within a wizard.
/// </summary>
/// <param name="message">An optional message to show; <see langword="null"/> shows none.</param>
[ExcludeFromCodeCoverage]
public class SaveSuccessResult(string? message = null) : ISaveResult
{
    /// <summary>
    /// Gets the optional message to show.
    /// </summary>
    public string? Message => message;
}
