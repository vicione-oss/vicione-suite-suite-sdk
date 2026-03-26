using Sdk.Messaging;

namespace Sdk.SystemConfiguration.Events;

/// <summary>
/// Represents an event that is published when the system configuration has changed.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public record SystemConfigurationChanged() : ResponseEventBase;
