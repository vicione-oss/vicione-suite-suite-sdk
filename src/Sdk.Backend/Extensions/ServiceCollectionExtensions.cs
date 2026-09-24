using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Modules;
using Sdk.Backend.Persistence;
using Sdk.Modules;

namespace Sdk.Backend.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register backend-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a module's database context that dynamically switches between a SQLite and a PostgreSQL
        /// implementation based on the host application's configuration.
        /// </summary>
        /// <remarks>
        /// The host application must register an <see cref="IModuleDbContextRegistrar"/> implementation
        /// before modules call this method. The registrar provides the actual resolution strategy.
        /// </remarks>
        /// <param name="module">The owning module; it must provide a <see cref="BackendModule.ModuleInitializer"/>.</param>
        /// <param name="sqliteDbName">The SQLite database name; <see langword="null"/> means the module ID.</param>
        /// <param name="enableSynchronization">Whether the context's data is replicated between master and slaves.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the module has no initializer, <typeparamref name="TDbContextInterface"/> is not an interface, or no
        /// <see cref="IModuleDbContextRegistrar"/> is registered.
        /// </exception>
        public IServiceCollection
            AddModuleDbContext<TDbContextInterface, TSqliteImplementation, TPostgresImplementation>(
                BackendModule module,
                string? sqliteDbName = null,
                bool enableSynchronization = true)
            where TDbContextInterface : IModuleDbContext
            where TSqliteImplementation : DbContext, ISqliteDbContext, TDbContextInterface
            where TPostgresImplementation : DbContext, IPostgresDbContext, TDbContextInterface
        {
            if (module.ModuleInitializer is null)
            {
                throw new InvalidOperationException(
                    $"A database context {typeof(TDbContextInterface).Name} was registered, but no initializer is present in the {module.ModuleId}-module");
            }

            if (!typeof(TDbContextInterface).IsInterface)
                throw new InvalidOperationException($"{nameof(TDbContextInterface)} must be an interface type!");

            var registrar = services.FindRegistrar();

            registrar.Register<TDbContextInterface, TSqliteImplementation, TPostgresImplementation>(
                services,
                module.ModuleId,
                module.GetType(),
                sqliteDbName ?? module.ModuleId,
                enableSynchronization);

            return services;
        }

        /// <summary>
        /// Registers <typeparamref name="TOption"/> as options bound from the section named after the module's ID without dots,
        /// e.g. <c>ViciOneSuiteOee</c>; <paramref name="validateDataAnnotations"/> enables data-annotation validation.
        /// </summary>
        public IServiceCollection AddModuleSection<TOption>(IModule module, bool validateDataAnnotations = true)
            where TOption : class
            => services.AddModuleSection<TOption>(module.ModuleKey.ModuleId, validateDataAnnotations);

        /// <summary>
        /// Registers <typeparamref name="TOption"/> as options bound from the section named after <paramref name="moduleId"/>
        /// without dots, e.g. <c>ViciOneSuiteOee</c>; <paramref name="validateDataAnnotations"/> enables data-annotation validation.
        /// </summary>
        public IServiceCollection AddModuleSection<TOption>(string moduleId, bool validateDataAnnotations = true)
            where TOption : class
        {
            // Bash cannot set environment variables whose names contain dots, so the section key drops them.
            var options = services.AddOptions<TOption>()
                .BindConfiguration(moduleId.Replace(".", "", StringComparison.Ordinal));

            if (validateDataAnnotations)
                options.ValidateDataAnnotations();

            return services;
        }

        /// <summary>
        /// Adds an <see cref="IModuleHostRequestHandler"/> to the service collection. Use it to handle requests from the module
        /// host, such as restore operations.
        /// </summary>
        public IServiceCollection AddModuleHostRequestHandler<TRequestHandler>()
            where TRequestHandler : class, IModuleHostRequestHandler
            => services.AddTransient<IModuleHostRequestHandler, TRequestHandler>();

        private IModuleDbContextRegistrar FindRegistrar()
        {
            var descriptor = services.LastOrDefault(d => d.ServiceType == typeof(IModuleDbContextRegistrar)) ?? throw new InvalidOperationException(
                    $"No {nameof(IModuleDbContextRegistrar)} has been registered. " +
                    "The host application must register an implementation before modules can register database contexts.");

            // An instance registration already is the object the container hands out.
            if (descriptor.ImplementationInstance is IModuleDbContextRegistrar instance)
                return instance;

            // A type or factory registration is resolved once here and pinned as an instance below, so the container
            // later hands out the same object that has already received the registrations.
            var resolved = descriptor switch
            {
                { ImplementationType: not null } =>
                    (IModuleDbContextRegistrar)(Activator.CreateInstance(descriptor.ImplementationType)
                                                ?? throw new InvalidOperationException($"Failed to create {descriptor.ImplementationType.FullName}")),
                { ImplementationFactory: not null } =>
                    (IModuleDbContextRegistrar)descriptor.ImplementationFactory(EmptyServiceProvider.Instance),
                _ => throw new InvalidOperationException("Invalid service registration for IModuleDbContextRegistrar.")
            };

            services.Remove(descriptor);
            services.AddSingleton(resolved);

            return resolved;
        }
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public static readonly EmptyServiceProvider Instance = new();
        public object? GetService(Type serviceType) => null;
    }
}
