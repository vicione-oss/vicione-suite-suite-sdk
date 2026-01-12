namespace Sdk.Messaging;

/// <summary>
/// Represents detailed information about an error that occurred.
/// </summary>
public record ErrorInfo(int ErrorCode, string? Message);
