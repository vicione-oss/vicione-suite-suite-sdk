using Sdk.Modules;

namespace Sdk.Authorization;

/// <summary>
/// Provides helper methods to generate module-based authorization policies.
/// </summary>
public static class ModulePolicyProvider
{
    /// <summary>
    /// Gets a policy for the module implementing the specified type <typeparamref name="T"/>.
    /// </summary>
    public static string GetPolicy<T>(AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => GetPolicy(typeof(T), minimumAccessLevel, featureName);

    /// <summary>
    /// Gets a policy for the module implementing the specified <paramref name="moduleType"/>.
    /// </summary>
    public static string GetPolicy(Type moduleType, AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => GetPolicy(ModuleIdResolver.ResolveId(moduleType), minimumAccessLevel, featureName);

    /// <summary>
    /// Gets a policy name for the specified <paramref name="moduleId"/>.
    /// </summary>
    public static string GetPolicy(string moduleId, AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => $"{Constants.AuthorizationPolicyPrefix}_{moduleId}_{minimumAccessLevel}" + (featureName is null ? string.Empty : $"_{featureName}");
}
