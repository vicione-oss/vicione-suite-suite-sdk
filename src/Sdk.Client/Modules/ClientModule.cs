using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Modules;

namespace Sdk.Client.Modules;

/// <summary>
/// Represents a base class for a client-side module in the system.
/// </summary>
public abstract class ClientModule : IClientModule
{
    private string? _moduleId;

    /// <summary>
    /// Gets the module ID, resolved once from the assembly name by <see cref="ModuleIdResolver"/>.
    /// </summary>
    public string ModuleId
    {
        get
        {
            _moduleId ??= ModuleIdResolver.ResolveId(GetType());
            return _moduleId;
        }
    }

    /// <summary>
    /// Gets the delegate that registers the module's services; <see langword="null"/>, the default, registers none.
    /// </summary>
    public virtual Action<IServiceCollection>? Configure => default;

    /// <summary>
    /// Gets the delegate run once the service provider is built, e.g. to warm caches; <see langword="null"/>, the default, runs none.
    /// </summary>
    public virtual Func<IServiceProvider, Task>? InitializeServices => default;

    /// <summary>
    /// Gets the delegate run after a user is authenticated, e.g. to load user settings; <see langword="null"/>, the default, runs none.
    /// </summary>
    public virtual Func<IServiceProvider, ClaimsPrincipal, Task>? OnUserAuthenticated => default;

    /// <summary>
    /// Gets the key of <see cref="ModuleId"/> and <see cref="ModuleType.Client"/>.
    /// </summary>
    public ModuleKey ModuleKey => new()
    {
        ModuleId = ModuleId,
        ModuleType = ModuleType.Client
    };
}

