using Sdk.Messaging;

namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents the result of a connection test.
/// </summary>
[ExcludeFromCodeCoverage]
public record ConnectionTestResult(bool Success, ErrorInfo? ErrorInfo);
