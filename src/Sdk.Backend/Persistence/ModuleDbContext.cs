using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sdk.Backend.Persistence;

/// <summary>
/// An abstract base class for a module-specific Entity Framework Core database context.
/// </summary>
public abstract class ModuleDbContext(DbContextOptions options) : DbContext(options), IModuleDbContext
{
    /// <inheritdoc/>
    public DbContext Instance => this;

    /// <inheritdoc/>
    public abstract string DefaultSchemaName { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<Type> NotSynchronizedEntityTypes => [];

    /// <summary>
    /// Overrides the base model creation process to enforce a default schema and then calls <see cref="OnModuleModelCreating"/>.
    /// </summary>
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchemaName);

        OnModuleModelCreating(modelBuilder);
    }

    /// <summary>
    /// When overridden in a derived class, allows for further configuration of the model that is being built.
    /// </summary>
    /// <remarks>
    /// This method is called by the sealed <see cref="OnModelCreating"/> after the default schema has been set.
    /// </remarks>
    protected virtual void OnModuleModelCreating(ModelBuilder modelBuilder) { }

    /// <summary>
    /// Overrides the base convention configuration to apply provider-specific workarounds and then calls <see cref="OnConfigureConventions"/>.
    /// </summary>
    protected sealed override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Workaround for SQLite, which does not have a native DateTimeOffset type.
        if (Database.IsSqlite())
            configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToStringConverter>();

        OnConfigureConventions(configurationBuilder);
    }

    /// <summary>
    /// When overridden in a derived class, allows for further configuration of conventions.
    /// </summary>
    /// <remarks>
    /// This method is called by the sealed <see cref="ConfigureConventions"/> after provider-specific conventions have been applied.
    /// </remarks>
    protected virtual void OnConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }
}
