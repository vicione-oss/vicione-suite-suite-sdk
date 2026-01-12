using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Modules;
using Sdk.Backend.Persistence;
using Sdk.Modules;

namespace Sdk.Backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection
        AddDynamicDbContext<TDbContextBaseInterface, TSqliteImplementation, TPostgresImplementation>(
            this IServiceCollection services,
            BackendModule module,
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

    public static IServiceCollection AddModuleSection<TOption>(this IServiceCollection services, IModule module, bool validateDataAnnotations = true)
        where TOption : class
        => services.AddModuleSection<TOption>(module.ModuleKey.ModuleId, validateDataAnnotations);

    public static IServiceCollection AddModuleSection<TOption>(this IServiceCollection services, string moduleId, bool validateDataAnnotations = true)
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
    /// Adds a <see cref="IModuleHostRequestHandler"/> to service collection. Use it to handle requests from module
    /// host like restore.
    /// </summary>
    /// <typeparam name="TRequestHandler"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddModuleHostRequestHandler<TRequestHandler>(this IServiceCollection services)
        where TRequestHandler : class, IModuleHostRequestHandler
        => services.AddTransient<IModuleHostRequestHandler, TRequestHandler>();
}
