using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Persistence;

namespace Sdk.Backend.Modules;

/// <summary>
/// Provides a default implementation of <see cref="IModuleInitializer"/> for modules that use Entity Framework Core.
/// Handles database migration and exposes lifecycle hooks that can be overridden.
/// </summary>
/// <typeparam name="TDbContext">
/// The database context type used by the module, which must implement <see cref="IModuleDbContext"/>.
/// </typeparam>
public class DefaultModuleInitializer<TDbContext> : IModuleInitializer
    where TDbContext : IModuleDbContext
{
    /// <summary>
    /// Called before the database migration is started.
    /// Override to perform tasks that must happen before any migrations are applied.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services for pre-migration work.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals when the operation should be aborted.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Performs database migrations by resolving the configured <typeparamref name="TDbContext"/>
    /// and running <see cref="Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade.MigrateAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that is used to resolve the <typeparamref name="TDbContext"/>.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals when the operation should be aborted.
    /// </param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    public virtual async Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
    {
        var context = scopedServices.GetRequiredService<TDbContext>();
        await context.Instance.Database.MigrateAsync(stoppingToken);
    }

    /// <summary>
    /// Called after the database migration has completed.
    /// Override to perform tasks that must happen after migrations are applied, such as seeding data.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services for post-migration work.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals when the operation should be aborted.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Called after the module has finished its initialization process.
    /// Override to perform any additional startup logic for your module.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services for initialization.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals when the operation should be aborted.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default)
        => Task.CompletedTask;
}
