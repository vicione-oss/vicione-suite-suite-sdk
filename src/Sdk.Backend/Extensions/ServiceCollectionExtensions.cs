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
        /// Binds a configuration section, identified by the module's ID, to a <typeparamref name="TOption"/> object and registers it with the dependency injection container.
        /// </summary>
        public IServiceCollection AddModuleSection<TOption>(IModule module, bool validateDataAnnotations = true)
            where TOption : class
            => services.AddModuleSection<TOption>(module.ModuleKey.ModuleId, validateDataAnnotations);

        /// <summary>
        /// Binds a configuration section, identified by the module ID, to a <typeparamref name="TOption"/> object and registers it with the dependency injection container.
        /// </summary>
        public IServiceCollection AddModuleSection<TOption>(string moduleId, bool validateDataAnnotations = true)
            where TOption : class
        {
            // module id is like 'ViciOne.Suite.Oee' but bash does not support setting environment variables with dots
            // therefore our section key need to be transformed to 'ViciOneSuiteOee'  
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
            var descriptor = services.LastOrDefault(d => d.ServiceType == typeof(IModuleDbContextRegistrar));

            return descriptor?.ImplementationInstance as IModuleDbContextRegistrar
                   ?? throw new InvalidOperationException(
                       $"No {nameof(IModuleDbContextRegistrar)} has been registered. " +
                       "The host application must register an implementation before modules can register database contexts.");
        }
    }
}
