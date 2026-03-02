namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// Represents the result of a failed save operation.
/// </summary>
[ExcludeFromCodeCoverage]
public class SaveErrorResult(string message, int? errorCode = null) : ISaveResult
{
    /// <summary>
    /// The error message describing the reason for the failure.
    /// </summary>
    public string Message => message;

    /// <summary>
    /// Gets the optional error code associated with the failure.
    /// </summary>
    public int? ErrorCode => errorCode;
}
