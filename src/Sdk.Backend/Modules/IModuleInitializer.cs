namespace Sdk.Backend.Modules;

/// <summary>
/// Startup hooks of a module, called in the order <see cref="OnPreMigrate"/>, <see cref="Migrate"/>,
/// <see cref="OnPostMigrate"/>, <see cref="OnInitialized"/>, each with services from one scope.
/// </summary>
public interface IModuleInitializer
{
    /// <summary>
    /// Runs before migration, e.g. to validate configuration or create required directories.
    /// </summary>
    Task OnPreMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Brings the module's database schema up to date, typically by applying EF Core migrations.
    /// </summary>
    Task Migrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Runs after a successful migration, e.g. to seed data or build caches.
    /// </summary>
    Task OnPostMigrate(IServiceProvider scopedServices, CancellationToken stoppingToken = default);

    /// <summary>
    /// Runs last, once the module is fully initialized.
    /// </summary>
    Task OnInitialized(IServiceProvider scopedServices, CancellationToken stoppingToken = default);
}
