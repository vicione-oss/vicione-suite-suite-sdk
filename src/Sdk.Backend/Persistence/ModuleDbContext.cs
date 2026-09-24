using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Base class of a module's EF Core context. Mark the derived class with <see cref="ModuleDbContextAttribute"/> to generate
/// its SQLite and PostgreSQL implementations.
/// </summary>
public abstract class ModuleDbContext(DbContextOptions options) : DbContext(options), IModuleDbContext
{
    /// <inheritdoc/>
    public abstract string DefaultSchemaName { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<Type> NotSynchronizedEntityTypes => [];

    /// <inheritdoc/>
    public Task MigrateAsync(CancellationToken cancellationToken = default)
        => Database.MigrateAsync(cancellationToken);

    /// <summary>
    /// Sets <see cref="DefaultSchemaName"/> as the default schema, then calls <see cref="OnModuleModelCreating"/>.
    /// </summary>
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchemaName);

        OnModuleModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the module's model, in place of <see cref="OnModelCreating"/>; the default schema is already set.
    /// </summary>
    protected virtual void OnModuleModelCreating(ModelBuilder modelBuilder) { }

    /// <summary>
    /// Stores <see cref="DateTimeOffset"/> as text on SQLite, then calls <see cref="OnConfigureConventions"/>.
    /// </summary>
    protected sealed override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Workaround for SQLite, which does not have a native DateTimeOffset type.
        if (Database.IsSqlite())
            configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToStringConverter>();

        OnConfigureConventions(configurationBuilder);
    }

    /// <summary>
    /// Configures the module's conventions, in place of <see cref="ConfigureConventions"/>; provider workarounds are already applied.
    /// </summary>
    protected virtual void OnConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }
}
