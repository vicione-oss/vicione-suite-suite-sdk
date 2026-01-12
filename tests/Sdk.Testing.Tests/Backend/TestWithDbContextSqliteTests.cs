using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Persistence;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class TestWithDbContextSqliteTests : TestWithDbContextSqlite<TheDbContextSqlite>
{
    [Fact]
    public void Should_create_an_in_memory_instance()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddScoped(_ => (ITheDbContext)TestDbContext)
            .BuildServiceProvider();

        // Act
        using var dbContext = services.GetRequiredService<ITheDbContext>();

        // Assert
        dbContext.Db.Should().NotBeNull();
    }
}

public interface ITheDbContext : IModuleDbContext
{
    DbContext Db { get; }
}

public class TheDbContext : ModuleDbContext, ITheDbContext
{
    internal const string DbSchemaName = "tests";
    public override string DefaultSchemaName => DbSchemaName;

    public DbContext Db => Instance;

    internal TheDbContext(DbContextOptions options)
        : base(options)
    {
    }
}

public sealed class TheDbContextSqlite(DbContextOptions<TheDbContextSqlite> options) : TheDbContext(options), ISqliteDbContext;
