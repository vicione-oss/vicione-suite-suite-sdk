using AwesomeAssertions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;
using Sdk.Backend.Persistence;
using TestModule.Backend;
using TestModule.Backend.DbContext;
using Xunit;

namespace Sdk.Backend.Tests.Persistence;

public sealed class DbContextResolverTests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    [Fact]
    public void Should_resolve_to_sqlite_on_systems_that_are_no_master()
    {
        // Arrange
        var options = new DbContextResolverOptions<ITestModuleDbContext>(typeof(TestBackendModule), "Test", false);
        var resolver = new DbContextResolver<TestModuleDbContextSqlite, TestModuleDbContextPostgres, ITestModuleDbContext>(options);
        var provider = _services
            .AddSingleton(Substitute.For<IWorkspaceProvider<TestBackendModule>>())
            .BuildServiceProvider();

        // Act
        var resolved = resolver.Resolve(provider);

        // Assert
        resolved.GetType().Should().Be<TestModuleDbContextSqlite>();
    }

    [Fact]
    public void Should_resolve_to_postgres_on_systems_that_master()
    {
        // Arrange
        var options = new DbContextResolverOptions<ITestModuleDbContext>(typeof(TestBackendModule), "Test", false);
        var resolver = new DbContextResolver<TestModuleDbContextSqlite, TestModuleDbContextPostgres, ITestModuleDbContext>(options);
        var provider = _services
            .AddSingleton(Substitute.For<IMasterDbConnectionStringProvider>())
            .AddSingleton(Substitute.For<IWorkspaceProvider<TestBackendModule>>())
            .BuildServiceProvider();

        // Act
        var resolved = resolver.Resolve(provider);

        // Assert
        resolved.GetType().Should().Be<TestModuleDbContextPostgres>();
    }

    [Fact]
    public void Should_add_db_interceptor_on_master_if_synchronization_is_enabled()
    {
        // Arrange
        var options = new DbContextResolverOptions<ITestModuleDbContext>(typeof(TestBackendModule), "Test");
        var resolver = new DbContextResolver<TestModuleDbContextSqlite, TestModuleDbContextPostgres, ITestModuleDbContext>(options);
        var provider = _services
            .AddSingleton(Substitute.For<IMasterDbConnectionStringProvider>())
            .AddSingleton(Substitute.For<ISaveChangesInterceptor>())
            .AddSingleton(Substitute.For<IWorkspaceProvider<TestBackendModule>>())
            .BuildServiceProvider();

        // Act
        var resolved = resolver.Resolve(provider);

        // Assert
        resolved.GetType().Should().Be<TestModuleDbContextPostgres>();
    }
}
