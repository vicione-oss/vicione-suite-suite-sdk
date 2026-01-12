using Sdk.Messaging;

namespace Sdk.Connections.Contracts;

public record ConnectionTestResult(bool Success, ErrorInfo? ErrorInfo);
