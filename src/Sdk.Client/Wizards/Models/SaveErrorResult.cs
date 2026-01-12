namespace Sdk.Client.Wizards.Models;

/// <summary>
/// Represents the result of a failed save operation within a wizard.
/// </summary>
public class SaveErrorResult(string message, int? errorCode = null) : ISaveResult
{
    /// <summary>
    /// The error message describing the reason for the failure.
    /// </summary>
    public string Message => message;

    /// <summary>
    /// An optional numerical code associated with the error.
    /// </summary>
    public int? ErrorCode => errorCode;
}
