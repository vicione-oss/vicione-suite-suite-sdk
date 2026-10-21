using Microsoft.AspNetCore.Authorization;
using Sdk.Modules;

namespace Sdk.Authorization;

/// <summary>
/// Requires the user to hold at least the given access level for a module or one of its features.
/// </summary>
/// <remarks>
/// Make sure to register the associated feature using
/// <see cref="Extensions.IServiceCollectionExtensions.AddModuleFeature"/> when using <paramref name="featureName"/>.
/// </remarks>
[method: SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "The arguments only build the policy, which AuthorizeAttribute.Policy exposes.")]
public class ModuleAuthorizeAttribute(string moduleId, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
    : AuthorizeAttribute(ModulePolicyProvider.GetPolicy(moduleId, accessLevel, featureName))
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleAuthorizeAttribute"/> class.
    /// </summary>
    [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "The arguments only build the policy, which AuthorizeAttribute.Policy exposes.")]
    public ModuleAuthorizeAttribute(Type moduleType, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
        : this(ModuleIdResolver.ResolveId(moduleType), accessLevel, featureName)
    { }
}

/// <inheritdoc />
[method: SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "The arguments only build the policy, which AuthorizeAttribute.Policy exposes.")]
public sealed class ModuleAuthorizeAttribute<T>(AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null) :
    ModuleAuthorizeAttribute(typeof(T), accessLevel, featureName)
    where T : IModule;
