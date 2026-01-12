using Sdk.Modules;

namespace Sdk.Authorization;

public static class ModulePolicyProvider
{
    public static string GetPolicy<T>(AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => GetPolicy(typeof(T), minimumAccessLevel, featureName);

    public static string GetPolicy(Type moduleType, AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => GetPolicy(ModuleIdResolver.ResolveId(moduleType), minimumAccessLevel, featureName);

    public static string GetPolicy(string moduleId, AccessLevel minimumAccessLevel = AccessLevel.Partial, string? featureName = null)
        => $"{Constants.AuthorizationPolicyPrefix}_{moduleId}_{minimumAccessLevel}" + (featureName is null ? string.Empty : $"_{featureName}");
}
