using Microsoft.EntityFrameworkCore;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Represents a database context that is specific to a module,
/// exposing the underlying <see cref="DbContext"/> instance along with metadata
/// such as default schema and entities that are excluded from synchronization.
/// </summary>
public interface IModuleDbContext : IDisposable
{
    /// <summary>
    /// Gets the underlying <see cref="DbContext"/> instance used by this module.
    /// </summary>
    /// <remarks>
    /// This property provides direct access to Entity Framework Core features such as
    /// querying, migrations, and transaction management.
    /// </remarks>
    DbContext Instance { get; }

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
}
