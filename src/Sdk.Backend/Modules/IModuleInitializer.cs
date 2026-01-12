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
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services
    /// needed during pre-migration tasks.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals if the operation should be aborted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Executes the database migration logic.
    /// This is typically where Entity Framework migrations or other schema updates are performed.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve the database context or other services
    /// necessary to perform the migration.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals if the operation should be aborted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous migration operation.
    /// </returns>
    Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Called after the migration step has successfully completed.
    /// Override or implement this method to perform post-migration tasks,
    /// such as seeding data, building caches, or initializing lookup tables.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services
    /// needed during post-migration tasks.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals if the operation should be aborted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Called after the module has been fully initialized.
    /// Override or implement this method to perform any final startup logic
    /// that should occur once migrations and configuration have finished.
    /// </summary>
    /// <param name="scopedServices">
    /// A scoped <see cref="IServiceProvider"/> that can be used to resolve services
    /// for the initialization step.
    /// </param>
    /// <param name="stoppingToken">
    /// A cancellation token that signals if the operation should be aborted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// By default, returns a completed task.
    /// </returns>
    Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default)
        => Task.CompletedTask;
}

