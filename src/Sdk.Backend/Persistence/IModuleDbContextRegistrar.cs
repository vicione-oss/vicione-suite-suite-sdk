using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Defines the strategy for registering a module's database context implementations with the dependency injection container.
/// </summary>
/// <remarks>
/// This interface is implemented by the host application (e.g., ViciOne Suite) to provide
/// the actual database context resolution logic (SQLite vs. PostgreSQL selection, connection string management,
/// interceptor wiring, etc.). The SDK invokes this registrar from its
/// <c>AddModuleDbContext</c> extension method, keeping the SDK free of implementation details.
/// </remarks>
public interface IModuleDbContextRegistrar
{
    /// <summary>
    /// Registers the database context implementations for a module into the service collection.
    /// </summary>
    /// <typeparam name="TDbContextInterface">The module-specific <see cref="IModuleDbContext"/> interface.</typeparam>
    /// <typeparam name="TSqliteImplementation">The SQLite implementation of the database context.</typeparam>
    /// <typeparam name="TPostgresImplementation">The PostgreSQL implementation of the database context.</typeparam>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="moduleId">The unique identifier of the module.</param>
    /// <param name="moduleType">The <see cref="Type"/> of the module that owns the database context.</param>
    /// <param name="sqliteDbName">The SQLite database file name.</param>
    /// <param name="enableSynchronization">Whether to enable data synchronization (replication) for this context.</param>
    void Register<TDbContextInterface, TSqliteImplementation, TPostgresImplementation>(
        IServiceCollection services,
        string moduleId,
        Type moduleType,
        string sqliteDbName,
        bool enableSynchronization)
        where TDbContextInterface : IModuleDbContext
        where TSqliteImplementation : DbContext, ISqliteDbContext, TDbContextInterface
        where TPostgresImplementation : DbContext, IPostgresDbContext, TDbContextInterface;
}

