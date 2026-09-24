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
        dbContext.ChangeTracker.Should().NotBeNull();
        dbContext.DefaultSchemaName.Should().Be(TheDbContext.DbSchemaName);
    }
}

public class TestWithDbContextSqliteDisposeTests
{
    [Fact]
    public async Task Should_run_both_hooks_once_when_disposed_asynchronously()
    {
        // Arrange
        var test = new RecordingTest();

        // Act
        await test.DisposeAsync();
        await test.DisposeAsync();

        // Assert
        test.Calls.Should().Equal("DisposeAsyncCore", "Dispose(False)");
    }

    [Fact]
    public void Should_run_the_sync_hook_once_when_disposed_synchronously()
    {
        // Arrange
        var test = new RecordingTest();

        // Act
        test.Dispose();
        test.Dispose();

        // Assert
        test.Calls.Should().Equal("Dispose(True)");
    }

    private sealed class RecordingTest : TestWithDbContextSqlite<TheDbContextSqlite>
    {
        public List<string> Calls { get; } = [];

        protected override async ValueTask DisposeAsyncCore()
        {
            Calls.Add(nameof(DisposeAsyncCore));
            await base.DisposeAsyncCore();
        }

        protected override void Dispose(bool disposing)
        {
            Calls.Add($"Dispose({disposing})");
            base.Dispose(disposing);
        }
    }
}

public interface ITheDbContext : IModuleDbContext;

public class TheDbContext : ModuleDbContext, ITheDbContext
{
    internal const string DbSchemaName = "tests";
    public override string DefaultSchemaName => DbSchemaName;


    internal TheDbContext(DbContextOptions options)
        : base(options)
    {
    }
}

public sealed class TheDbContextSqlite(DbContextOptions<TheDbContextSqlite> options) : TheDbContext(options), ISqliteDbContext;
