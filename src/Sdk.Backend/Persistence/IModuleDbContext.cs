using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Represents a database context that is specific to a module,
/// providing access to change tracking, persistence operations, migration,
/// and metadata such as default schema and entities excluded from synchronization.
/// </summary>
public interface IModuleDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker"/> instance
    /// for tracking entity state changes in this context.
    /// </summary>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Gets the <see cref="DatabaseFacade"/> for this context, providing access to
    /// database-related operations such as connection management, transactions, and raw SQL execution.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Gets the default schema name used by this module's database objects.
    /// </summary>
    /// <remarks>
    /// This schema name may be used during migrations or when generating queries
    /// to ensure objects are created in or resolved from the correct schema.
    /// </remarks>
    string DefaultSchemaName { get; }

    /// <summary>
    /// Gets a collection of entity types that should be excluded from schema synchronization or migration.
    /// </summary>
    /// <remarks>
    /// These types are excluded from the automatic synchronization between master and slave databases.
    /// </remarks>
    IEnumerable<Type> NotSynchronizedEntityTypes { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies any pending migrations for this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
