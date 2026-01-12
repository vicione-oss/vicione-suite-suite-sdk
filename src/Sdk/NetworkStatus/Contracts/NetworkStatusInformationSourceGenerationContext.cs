using System.Text.Json.Serialization;

namespace Sdk.NetworkStatus.Contracts;

/// <summary>
/// Represents the <see cref="NetworkStatusInformationSourceGenerationContext"/>.
/// </summary>
[JsonSourceGenerationOptions(
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(NetworkStatusInformation))]
public partial class NetworkStatusInformationSourceGenerationContext : JsonSerializerContext;
