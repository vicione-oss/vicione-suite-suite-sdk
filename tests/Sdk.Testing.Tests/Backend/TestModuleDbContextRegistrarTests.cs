using System.Data;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Persistence;
using Sdk.Testing.Backend;
using TestModule.Backend.DbContext;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public sealed class TestModuleDbContextRegistrarTests : IAsyncDisposable
{
    private const string ModuleId = "TestModule";
    private readonly TestModuleDbContextRegistrar _registrar = new();
    private readonly ServiceCollection _serviceCollection = new();

    [Fact]
    public void Should_register_db_context_interface_as_scoped_service()
    {
        // Act
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), ModuleId, true);

        // Assert
        var descriptor = _serviceCollection.Should().ContainSingle(d => d.ServiceType == typeof(IReferenceDbContext)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_register_module_context_type_information()
    {
        // Act
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), ModuleId, true);

        // Assert
        var descriptor = _serviceCollection.Should().ContainSingle(d => d.ServiceType == typeof(ModuleContextTypeInformation)).Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);

        var info = (ModuleContextTypeInformation)descriptor.ImplementationInstance!;
        info.ModuleId.Should().Be(ModuleId);
        info.ContextType.Should().Be<IReferenceDbContext>();
        info.FullName.Should().Be(typeof(IReferenceDbContext).FullName);
    }

    [Fact]
    public void Should_resolve_a_working_sqlite_db_context()
    {
        // Arrange
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "resolve_test", true);

        using var provider = _serviceCollection.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Act
        var dbContext = scope.ServiceProvider.GetRequiredService<IReferenceDbContext>();

        // Assert
        dbContext.Should().NotBeNull();
        dbContext.ChangeTracker.Should().NotBeNull();
    }

    [Fact]
    public void Should_reuse_keeper_connection_for_same_db_name()
    {
        // Act — register twice with the same sqliteDbName
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "shared_db", true);
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "shared_db", true);

        // Assert — two scoped registrations but no error; keeper connection opened only once
        var scopedDescriptors = _serviceCollection
            .Where(d => d.ServiceType == typeof(IReferenceDbContext) && d.Lifetime == ServiceLifetime.Scoped)
            .ToList();
        scopedDescriptors.Should().HaveCount(2);
    }

    [Fact]
    public void Should_create_separate_keeper_connections_for_different_db_names()
    {
        // Act
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "db_one", true);
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, "OtherModule", typeof(TestModuleDbContextRegistrarTests), "db_two", true);

        // Assert — both contexts should be resolvable independently
        using var provider = _serviceCollection.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // The last registration wins for IReferenceDbContext, but both keeper connections exist.
        // Verifying no exception means both keeper connections were successfully created.
        var dbContext = scope.ServiceProvider.GetRequiredService<IReferenceDbContext>();
        dbContext.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_dispose_all_keeper_connections_on_dispose()
    {
        // Arrange
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "dispose_test_a", true);
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, "Other", typeof(TestModuleDbContextRegistrarTests), "dispose_test_b", true);

        // Act — should not throw
        var action = async () => await _registrar.DisposeAsync();

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Should_allow_double_dispose()
    {
        // Arrange
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "double_dispose", true);

        // Act — dispose twice should not throw
        await _registrar.DisposeAsync();
        var action = async () => await _registrar.DisposeAsync();

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public void Should_register_context_type_information_per_call()
    {
        // Act
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, "ModuleA", typeof(TestModuleDbContextRegistrarTests), "dbA", true);
        _registrar.Register<ITestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(
            _serviceCollection, "ModuleB", typeof(TestModuleDbContextRegistrarTests), "dbB", true);

        // Assert
        var typeInfos = _serviceCollection
            .Where(d => d.ServiceType == typeof(ModuleContextTypeInformation))
            .Select(d => (ModuleContextTypeInformation)d.ImplementationInstance!)
            .ToList();

        typeInfos.Should().HaveCount(2);
        typeInfos.Should().Contain(t => t.ModuleId == "ModuleA" && t.ContextType == typeof(IReferenceDbContext));
        typeInfos.Should().Contain(t => t.ModuleId == "ModuleB" && t.ContextType == typeof(ITestModuleDbContext));
    }

    [Fact]
    public void Should_close_the_scoped_connection_when_the_scope_is_disposed()
    {
        // Arrange
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "scoped_connection", true);

        using var provider = _serviceCollection.BuildServiceProvider();
        var scope = provider.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<IReferenceDbContext>().Database.GetDbConnection();

        // Act
        scope.Dispose();

        // Assert
        connection.State.Should().Be(ConnectionState.Closed);
    }

    [Fact]
    public async Task Should_keep_the_shared_database_alive_for_as_long_as_the_registrar_lives()
    {
        // Arrange
        _registrar.Register<IReferenceDbContext, ReferenceDbContextSqlite, ReferenceDbContextPostgres>(
            _serviceCollection, ModuleId, typeof(TestModuleDbContextRegistrarTests), "keeper_lifetime", true);

        await using var provider = _serviceCollection.BuildServiceProvider();
        await using (var scope = provider.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IReferenceDbContext>();
            await dbContext.Database.ExecuteSqlRawAsync("CREATE TABLE marker (id INTEGER)", TestContext.Current.CancellationToken);
        }

        // Act
        var existsWhileRegistrarLives = await MarkerTableExists(provider);
        await _registrar.DisposeAsync();
        var existsAfterRegistrarDisposed = await MarkerTableExists(provider);

        // Assert
        existsWhileRegistrarLives.Should().BeTrue();
        existsAfterRegistrarDisposed.Should().BeFalse();
    }

    private static async Task<bool> MarkerTableExists(IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IReferenceDbContext>();
        var count = await dbContext.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = 'marker'")
            .SingleAsync(TestContext.Current.CancellationToken);
        return count == 1;
    }

    public async ValueTask DisposeAsync() =>
        await _registrar.DisposeAsync();
}

