using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Source-generated JSON metadata for <see cref="SystemConfiguration"/>.
/// </summary>
[JsonSourceGenerationOptions(
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(SystemConfiguration))]
public partial class SystemConfigurationSourceGenerationContext : JsonSerializerContext;
