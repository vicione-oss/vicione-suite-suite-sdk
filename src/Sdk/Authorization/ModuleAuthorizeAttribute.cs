using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Sdk.Modules;

namespace Sdk.Authorization;

/// <summary>
///     Authorizes a part of the application based on the users permissions. Make sure to register a feature using
///     <see cref="Sdk.Authorization.Extensions.IServiceCollectionExtensions.AddModuleFeature"/> when using the featureName parameter. 
/// </summary>
[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
public class ModuleAuthorizeAttribute(string moduleId, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
    : AuthorizeAttribute(ModulePolicyProvider.GetPolicy(moduleId, accessLevel, featureName))
{
    public ModuleAuthorizeAttribute(Type moduleType, AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null)
        : this(ModuleIdResolver.ResolveId(moduleType), accessLevel, featureName)
    { }
}

/// <inheritdoc />
[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
public sealed class ModuleAuthorizeAttribute<T>(AccessLevel accessLevel = AccessLevel.Partial, string? featureName = null) :
    ModuleAuthorizeAttribute(typeof(T), accessLevel, featureName)
    where T : IModule;
