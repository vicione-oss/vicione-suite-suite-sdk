using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Instance;
using Sdk.Modules;

namespace Sdk.Backend.Modules;

/// <summary>
/// Base class of a backend module. The host calls its hooks in the order <see cref="ConfigureServices"/>,
/// <see cref="ConfigureMessageBus"/>, <see cref="UseServices"/>, <see cref="MapEndpoints"/>; all default to doing nothing.
/// </summary>
public abstract class BackendModule : IModule
{
    private string? _moduleId;

    /// <summary>
    /// Gets the initializer that runs the module's startup logic, such as migrations; <see langword="null"/>, the default, means none.
    /// A module that calls <c>AddModuleDbContext</c> must provide one.
    /// </summary>
    public virtual IModuleInitializer? ModuleInitializer => null;

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
    /// Gets the key of <see cref="ModuleId"/> and <see cref="ModuleType.Backend"/>; two modules are equal when their keys are.
    /// </summary>
    public ModuleKey ModuleKey => new()
    {
        ModuleId = ModuleId,
        ModuleType = ModuleType.Backend,
    };

    /// <summary>
    /// Gets whether the module is created without a default feature. Defaults to <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// Without a default feature, every access check of the module must name a feature explicitly.
    /// </remarks>
    public virtual bool DisableDefaultFeature => false;

    /// <summary>
    /// Returns which files are deployed to the module's workspace directory and how they are copied;
    /// <see langword="null"/>, the default, deploys none.
    /// </summary>
    public virtual ModuleResourceOptions? GetResourceOptions(IServiceProvider services) => null;

    /// <summary>
    /// Registers the module's services and MVC parts. Called first, before the message bus is configured.
    /// </summary>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder) { }

    /// <summary>
    /// Registers the module's consumers, activities and sagas.
    /// </summary>
    /// <param name="busConfig">
    /// An <see cref="IServiceCollection"/> instance that can be cast to an <c>IBusRegistrationConfigurator</c>.
    /// This indirection is used because the relevant interface may not be present in MassTransit.Abstractions package yet.
    /// </param>
    /// <param name="instanceType">The type of the running instance, e.g. to register a consumer on the master only.</param>
    public virtual void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType) { }

    /// <summary>
    /// Adds the module's middleware to the request pipeline; the service provider is built at this point.
    /// </summary>
    public virtual void UseServices(IApplicationBuilder app) { }

    /// <summary>
    /// Maps the module's HTTP endpoints. Called last.
    /// </summary>
    public virtual void MapEndpoints(IEndpointRouteBuilder endpoints) { }

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => ModuleKey.Equals((obj as BackendModule)?.ModuleKey);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => HashCode.Combine(ModuleKey);
}
