namespace Sdk.Client.Wizards.Models;

/// <inheritdoc cref="ISaveResult"/>
/// <param name="message">Success message</param>
[ExcludeFromCodeCoverage]
public class SaveSuccessResult(string? message = null) : ISaveResult
{
    /// <summary>
    /// Success message
    /// </summary>
    public string? Message => message;
}
