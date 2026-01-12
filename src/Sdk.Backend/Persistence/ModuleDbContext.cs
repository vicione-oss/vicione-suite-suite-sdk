using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sdk.Backend.Persistence;

public abstract class ModuleDbContext(DbContextOptions options) : DbContext(options), IModuleDbContext
{
    public DbContext Instance => this;
    public abstract string DefaultSchemaName { get; }
    public virtual IEnumerable<Type> NotSynchronizedEntityTypes => [];

    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchemaName);

        OnModuleModelCreating(modelBuilder);
    }

    protected virtual void OnModuleModelCreating(ModelBuilder modelBuilder) { }

    protected sealed override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        if (Database.IsSqlite())
            configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToStringConverter>();

        OnConfigureConventions(configurationBuilder);
    }

    protected virtual void OnConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }
}
