using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Persistence;

namespace Sdk.Backend.Modules;

/// <summary>
/// Provides a default implementation of <see cref="IModuleInitializer"/> for modules that use Entity Framework Core.
/// Handles database migration and exposes lifecycle hooks that can be overridden.
/// </summary>
public class DefaultModuleInitializer<TDbContext> : IModuleInitializer
    where TDbContext : IModuleDbContext
{
    /// <summary>
    /// Called before the database migration is started.
    /// Override to perform tasks that must happen before any migrations are applied.
    /// </summary>
    public virtual Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Performs database migrations by resolving the configured <typeparamref name="TDbContext"/>.
    /// </summary>
    public virtual async Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
    {
        var context = scopedServices.GetRequiredService<TDbContext>();

        await context.MigrateAsync(stoppingToken);
    }

    /// <summary>
    /// Called after the database migration has completed.
    /// Override to perform tasks that must happen after migrations are applied, such as seeding data.
    /// </summary>
    public virtual Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Called after the module has finished its initialization process.
    /// Override to perform any additional startup logic for your module.
    /// </summary>
    public virtual Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default)
        => Task.CompletedTask;
}
