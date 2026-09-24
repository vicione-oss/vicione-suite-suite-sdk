namespace Sdk.Client.Wizards.Models;

/// <summary>
/// Represents the result of a failed save operation within a wizard.
/// </summary>
[ExcludeFromCodeCoverage]
public class SaveErrorResult(string message, int? errorCode = null) : ISaveResult
{
    /// <summary>
    /// Gets the message describing why the save failed.
    /// </summary>
    public string Message => message;

    /// <summary>
    /// Gets the optional error code; <see langword="null"/> if there is none.
    /// </summary>
    public int? ErrorCode => errorCode;
}
