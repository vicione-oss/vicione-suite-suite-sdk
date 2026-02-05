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
        /// Registers a database context that can dynamically switch between a SQLite and a PostgreSQL implementation based on the application's configuration.
        /// </summary>
        public IServiceCollection
            AddDynamicDbContext<TDbContextBaseInterface, TSqliteImplementation, TPostgresImplementation>(BackendModule module,
                string? sqliteDbName = null,
                bool enableSynchronization = true)
            where TDbContextBaseInterface : IModuleDbContext
            where TSqliteImplementation : DbContext, ISqliteDbContext, TDbContextBaseInterface
            where TPostgresImplementation : DbContext, IPostgresDbContext, TDbContextBaseInterface
        {
            if (module.ModuleInitializer is null)
            {
                throw new InvalidOperationException(
                    $"A database context {typeof(TDbContextBaseInterface).Name} was registered, but no initializer is present in the {module.ModuleId}-module");
            }

            if (!typeof(TDbContextBaseInterface).IsInterface)
                throw new InvalidOperationException($"{nameof(TDbContextBaseInterface)} must be an interface type!");

            services.AddSingleton(
                new DbContextResolverOptions<TDbContextBaseInterface>(module.GetType(),
                    sqliteDbName ?? module.ModuleId,
                    enableSynchronization));

            services.AddTransient<DbContextResolver<TSqliteImplementation, TPostgresImplementation, TDbContextBaseInterface>>();
            services.AddSingleton(new ModuleContextTypeInformation(module.ModuleId,
                typeof(TDbContextBaseInterface),
                typeof(TDbContextBaseInterface).FullName!));

            services.AddScoped(typeof(TDbContextBaseInterface),
                s => s.GetRequiredService<DbContextResolver<TSqliteImplementation, TPostgresImplementation, TDbContextBaseInterface>>()
                    .Resolve(s));

            services.AddScoped(typeof(TPostgresImplementation).BaseType!,
                s => //allows to inject shared type - necessary for Sagas
                    s.GetRequiredService<DbContextResolver<TSqliteImplementation, TPostgresImplementation, TDbContextBaseInterface>>()
                        .Resolve(s));
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
    }
}
