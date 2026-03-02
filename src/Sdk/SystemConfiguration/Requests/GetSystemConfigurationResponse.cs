using Sdk.Messaging;

namespace Sdk.SystemConfiguration.Requests;

/// <summary>
/// Represents the response to a GetSystemConfiguration request.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GetSystemConfigurationResponse : IResponse
{
    /// <summary>
    /// Gets the system configuration if the request was successful.
    /// </summary>
    public Contracts.SystemConfiguration? Configuration { get; init; }

    /// <summary>
    /// Gets error information if the request failed, otherwise <see langword="null"/>.
    /// </summary>
    public ErrorInfo? RequestError { get; init; }
}
