using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Source-generated JSON metadata for <see cref="SystemConfiguration"/>.
/// </summary>
/// <remarks>
/// A member that is not required and is missing in the JSON is read with its default. A member this SDK version does not know,
/// such as one added by a newer version, is ignored.
/// </remarks>
[JsonSourceGenerationOptions(
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(SystemConfiguration))]
public partial class SystemConfigurationSourceGenerationContext : JsonSerializerContext;
