using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Persistence;

namespace Sdk.Backend.Modules;

/// <summary>
/// An <see cref="IModuleInitializer"/> that migrates <typeparamref name="TDbContext"/>; every other hook does nothing
/// until overridden.
/// </summary>
public class DefaultModuleInitializer<TDbContext> : IModuleInitializer
    where TDbContext : IModuleDbContext
{
    /// <summary>
    /// Does nothing; override to run code before migration.
    /// </summary>
    public virtual Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Applies the pending migrations of the <typeparamref name="TDbContext"/> resolved from <paramref name="scopedServices"/>.
    /// </summary>
    public virtual async Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
    {
        var context = scopedServices.GetRequiredService<TDbContext>();

        await context.MigrateAsync(stoppingToken);
    }

    /// <summary>
    /// Does nothing; override to run code after migration, e.g. to seed data.
    /// </summary>
    public virtual Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken)
        => Task.CompletedTask;

    /// <summary>
    /// Does nothing; override to run code once the module is fully initialized.
    /// </summary>
    public virtual Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default)
        => Task.CompletedTask;
}
