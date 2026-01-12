namespace Sdk.Messaging;

/// <summary>
/// Represents a marker interface for response messages in a request-response pattern.
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Gets error information if the request failed, otherwise <see langword="null"/>.
    /// </summary>
    ErrorInfo? RequestError { get; }
}
