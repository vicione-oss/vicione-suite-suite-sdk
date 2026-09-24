namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// Represents the result of a failed save operation.
/// </summary>
[ExcludeFromCodeCoverage]
public class SaveErrorResult(string message, int? errorCode = null) : ISaveResult
{
    /// <summary>
    /// Gets the message describing why the save failed.
    /// </summary>
    public string Message => message;

    /// <summary>
    /// Gets the optional error code associated with the failure.
    /// </summary>
    public int? ErrorCode => errorCode;
}
