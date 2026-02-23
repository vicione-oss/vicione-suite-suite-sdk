using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sdk.Backend.Persistence;
using TestModule.Backend.Contracts;
using TestModule.Backend.Helpers;

namespace TestModule.Backend.DbContext;

public class TestModuleDbContext(DbContextOptions options, List<SimpleDataTypes>? seedDataSimpleDataTypes = default) :
    ModuleDbContext(options), ITestModuleDbContext
{
    public const string Id = "TestModule";
    private readonly List<SimpleDataTypes>? _seedDataSimpleDataTypes = seedDataSimpleDataTypes ?? SeedData.DefaultSeedDataSimpleDataTypes();
    private readonly List<SimpleEmployees>? _seedDataSimpleEmployees = SeedData.DefaultDataSimpleEmployees();

    /// <inheritdoc/>
    public override string DefaultSchemaName => "datatypes";

    /// <inheritdoc/>
    public DbSet<SimpleDataTypes> SimpleDataTypes => Set<SimpleDataTypes>();

    /// <inheritdoc/>
    public DbSet<SimpleEmployees> Employees => Set<SimpleEmployees>();

    protected override void OnModuleModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SimpleEmployees>()
            .HasKey(e => e.emp_no);

        modelBuilder.Entity<SimpleEmployees>()
            .Property(e => e.Salutaion)
            .HasConversion<string>();

        modelBuilder.Entity<SimpleDataTypes>()
            .Property(e => e.Built).HasColumnType("TEXT");
        modelBuilder.Entity<SimpleDataTypes>()
            .Property(e => e.Created).HasColumnType("TEXT");

        modelBuilder.Entity<SimpleDataTypes>()
            .HasData(_seedDataSimpleDataTypes!);

        modelBuilder.Entity<SimpleEmployees>()
            .HasData(_seedDataSimpleEmployees!);
    }
}

public class DateTimeToIso8601StringConverter : ValueConverter<DateTimeOffset, string>
{
    private static readonly Expression<Func<string, DateTimeOffset>> s_deserialize = x => DateTime.Parse(x, new CultureInfo("de-DE")).ToUniversalTime();
    private static readonly Expression<Func<DateTimeOffset, string>> s_serialize = x => x.ToString("o", CultureInfo.InvariantCulture);

    public DateTimeToIso8601StringConverter() : base(s_serialize, s_deserialize)
    {
    }
}

public class TestModuleDbContextSqlite(DbContextOptions<TestModuleDbContextSqlite> options,
     List<SimpleDataTypes>? seedDataSimpleDataTypes = default) : TestModuleDbContext(options, seedDataSimpleDataTypes), ISqliteDbContext
{
    public const string DbName = "TestModule.db";
}

public class TestModuleDbContextPostgres(DbContextOptions<TestModuleDbContextPostgres> options,
    List<SimpleDataTypes>? seedDataSimpleDataTypes = default) : TestModuleDbContext(options, seedDataSimpleDataTypes), IPostgresDbContext;

public class TestModuleDbContextPostgresFactory : IDesignTimeDbContextFactory<TestModuleDbContextPostgres>
{
    public TestModuleDbContextPostgres CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestModuleDbContextPostgres>();
        optionsBuilder.UseNpgsql();
        return new TestModuleDbContextPostgres(optionsBuilder.Options);
    }
}

public class TestModuleDbContextSqliteFactory : IDesignTimeDbContextFactory<TestModuleDbContextSqlite>
{
    public TestModuleDbContextSqlite CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestModuleDbContextSqlite>();
        optionsBuilder.UseSqlite();
        return new TestModuleDbContextSqlite(optionsBuilder.Options);
    }
}
