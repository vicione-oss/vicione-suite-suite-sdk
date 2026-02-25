namespace Sdk.Backend.Modules;

/// <summary>
/// Defines lifecycle hooks for initializing a module that uses a database or other
/// startup routines requiring migration and configuration steps.
/// </summary>
public interface IModuleInitializer
{
    /// <summary>
    /// Called before any database migration is started.
    /// Override or implement this method to perform tasks that must occur
    /// prior to applying migrations, such as validating configuration,
    /// creating required directories, or preparing external resources.
    /// </summary>
    Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Executes the database migration logic.
    /// This is typically where Entity Framework migrations or other schema updates are performed.
    /// </summary>
    Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Called after the migration step has successfully completed.
    /// Override or implement this method to perform post-migration tasks,
    /// such as seeding data, building caches, or initializing lookup tables.
    /// </summary>
    Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Called after the module has been fully initialized.
    /// Override or implement this method to perform any final startup logic
    /// that should occur once migrations and configuration have finished.
    /// </summary>
    Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default);
}
