using Sdk.Messaging;

namespace Sdk.SystemConfiguration.Requests;

/// <summary>
/// Represents a parameterless request to retrieve the entire system configuration.
/// </summary>
public record GetSystemConfiguration : IRequest<GetSystemConfigurationResponse>;
