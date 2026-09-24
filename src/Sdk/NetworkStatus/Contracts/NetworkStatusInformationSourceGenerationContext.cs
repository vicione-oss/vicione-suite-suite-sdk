using System.Text.Json.Serialization;

namespace Sdk.NetworkStatus.Contracts;

/// <summary>
/// Source-generated JSON metadata for <see cref="NetworkStatusInformation"/>; unknown members are rejected.
/// </summary>
[JsonSourceGenerationOptions(
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(NetworkStatusInformation))]
public partial class NetworkStatusInformationSourceGenerationContext : JsonSerializerContext;
