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
    /// Gets the unique identifier for this client module.
    /// The value is lazily resolved from the module's <see cref="Type"/> using <see cref="ModuleIdResolver"/>.
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
    /// Allows the module to configure services based on the hosting model.
    /// </summary>
    [Obsolete("Support of WASM hosting model will be removed and therefore this method is obsolete. Use " + nameof(Configure) + " instead.")]
    public virtual Action<IServiceCollection, HostingModel>? ConfigureServices => default;

    /// <summary>
    /// Allows the module to configure services for the host application.
    /// Override this property to supply a delegate that registers
    /// services into the given <see cref="IServiceCollection"/>.
    /// </summary>
    public virtual Action<IServiceCollection>? Configure => default;

    /// <summary>
    /// Provides an optional asynchronous delegate that runs after the service provider has been built.
    /// Override this to perform module-specific initialization, such as
    /// seeding data or warming caches.
    /// </summary>
    public virtual Func<IServiceProvider, Task>? InitializeServices => default;

    /// <summary>
    /// Provides an optional asynchronous delegate that runs after a user has been authenticated.
    /// Override this to perform actions like loading user-specific settings or permissions.
    /// </summary>
    public virtual Func<IServiceProvider, ClaimsPrincipal, Task>? OnUserAuthenticated => default;

    /// <summary>
    /// Gets the <see cref="ModuleKey"/> representing this module,
    /// including its <see cref="ModuleId"/> and module type (<see cref="ModuleType.Client"/>).
    /// </summary>
    public ModuleKey ModuleKey => new()
    {
        ModuleId = ModuleId,
        ModuleType = ModuleType.Client
    };
}

