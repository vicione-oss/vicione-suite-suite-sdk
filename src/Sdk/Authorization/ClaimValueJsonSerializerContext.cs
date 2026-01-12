using System.Text.Json.Serialization;

namespace Sdk.Authorization;

/// <inheritdoc cref="JsonSerializerContext" />
[JsonSourceGenerationOptions(
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(ModuleAuthorizationClaimValue))]
internal partial class ClaimValueJsonSerializerContext : JsonSerializerContext;
