using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Instance;
using Sdk.Modules;

namespace Sdk.Backend.Modules;

/// <summary>
/// Represents a base class for a backend module in the system.
/// Provides default behavior and overridable hooks for service configuration,
/// message bus integration, endpoint mapping, and optional initialization logic.
/// </summary>
public abstract class BackendModule : IModule
{
    private string? _moduleId;

    /// <summary>
    /// Optional module initializer that can run startup logic when the module is loaded.
    /// Override this property to provide a custom <see cref="IModuleInitializer"/>.
    /// </summary>
    public virtual IModuleInitializer? ModuleInitializer => null;

    /// <summary>
    /// Unique identifier for this backend module.
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
    /// <see cref="ModuleKey"/> for this module, including its <see cref="ModuleId"/> and the <see cref="ModuleType.Backend"/>
    /// </summary>
    public ModuleKey ModuleKey => new()
    {
        ModuleId = ModuleId,
        ModuleType = ModuleType.Backend,
    };

    /// <summary>
    /// Allows disabling the default feature for this module.
    /// </summary>
    /// <remarks>
    /// If this is set to true, no default feature is created for the module.
    /// All access checks will therefore have to be tied to a named feature making this parameter required.
    /// </remarks>
    public virtual bool DisableDefaultFeature => false;

    /// <summary>
    /// Gets the optional directory containing embedded or static resources for this module.
    /// Override to return a path when your module needs to expose additional files.
    /// </summary>
    public virtual string? GetResourceDirectory(IServiceProvider services) => null;

    /// <summary>
    /// Configures services and MVC options for this module.
    /// Override this method to register module-specific services into the provided service collection.
    /// </summary>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder) { }

    /// <summary>
    /// Configures services specific to the message bus.
    /// </summary>
    /// <param name="busConfig">
    /// An <see cref="IServiceCollection"/> instance that can be cast to an <c>IBusRegistrationConfigurator</c>.
    /// This indirection is used because the relevant interface may not be present in MassTransit.Abstractions package yet.
    /// </param>
    /// <param name="instanceType">The <see cref="InstanceType"/> to allow conditional bus configuration.</param>
    public virtual void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType) { }

    /// <summary>
    /// Allows the module to configure middleware and request-handling services after they are built.
    /// Override this method to add middleware components to the application's request pipeline.
    /// </summary>
    public virtual void UseServices(IApplicationBuilder app) { }

    /// <summary>
    /// Allows the module to map its endpoints to the application's endpoint route builder.
    /// Override this method to register HTTP endpoints specific to this module.
    /// </summary>
    public virtual void MapEndpoints(IEndpointRouteBuilder endpoints) { }

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => ModuleKey.Equals((obj as BackendModule)?.ModuleKey);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => HashCode.Combine(ModuleKey);
}
