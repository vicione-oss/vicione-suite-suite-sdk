using Microsoft.AspNetCore.Authorization;
using Sdk.Modules;

namespace Sdk.Authorization;

/// <summary>
/// Authorizes a part of the application based on the users permissions.
/// </summary>
/// <remarks>
/// Make sure to register the associated feature using
/// <see cref="Extensions.IServiceCollectionExtensions.AddModuleFeature"/> when using <paramref name="featureName"/>.
/// </remarks>
[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
public class ModuleAuthorizeAttribute(string moduleId, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
    : AuthorizeAttribute(ModulePolicyProvider.GetPolicy(moduleId, accessLevel, featureName))
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleAuthorizeAttribute"/> class.
    /// </summary>
    public ModuleAuthorizeAttribute(Type moduleType, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
        : this(ModuleIdResolver.ResolveId(moduleType), accessLevel, featureName)
    { }
}

/// <inheritdoc />
[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
public sealed class ModuleAuthorizeAttribute<T>(AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null) :
    ModuleAuthorizeAttribute(typeof(T), accessLevel, featureName)
    where T : IModule;
