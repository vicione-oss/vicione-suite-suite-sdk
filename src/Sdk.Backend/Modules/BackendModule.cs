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
    public virtual IModuleInitializer? ModuleInitializer => default;

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
    /// Gets the optional directory containing embedded or static resources for this module.
    /// Override to return a path when your module needs to expose additional files.
    /// </summary>
    /// <param name="services">The service provider for resolving required services.</param>
    /// <returns>
    /// A string representing the resource directory path, or <see langword="null"/> if no resource directory is provided.
    /// </returns>
    public virtual string? GetResourceDirectory(IServiceProvider services) => null;

    /// <summary>
    /// Configures services and MVC options for this module.
    /// Override this method to register module-specific services into the provided service collection.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="config">The application configuration.</param>
    /// <param name="builder">The MVC builder to customize MVC-specific options.</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder) { }

    /// <summary>
    /// Configures services specific to the message bus (e.g., MassTransit).
    /// </summary>
    /// <param name="busConfig">
    /// An <see cref="IServiceCollection"/> instance that can be cast to an <c>IBusRegistrationConfigurator</c>.
    /// This indirection is used because the relevant interface may not be present in MassTransit.Abstractions package yet.
    /// </param>
    /// <param name="instanceType">
    /// The current <see cref="InstanceType"/> (e.g., development, staging, production) to allow conditional bus configuration.
    /// </param>
    public virtual void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType) { }

    /// <summary>
    /// Allows the module to configure middleware and request-handling services after they are built.
    /// Override this method to add middleware components to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public virtual void UseServices(IApplicationBuilder app) { }

    /// <summary>
    /// Allows the module to map its endpoints to the application's endpoint route builder.
    /// Override this method to register HTTP endpoints specific to this module.
    /// </summary>
    /// <param name="endpoints">The application's endpoint route builder.</param>
    public virtual void MapEndpoints(IEndpointRouteBuilder endpoints) { }

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => ModuleKey.Equals((obj as BackendModule)?.ModuleKey);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => HashCode.Combine(ModuleKey);
}
