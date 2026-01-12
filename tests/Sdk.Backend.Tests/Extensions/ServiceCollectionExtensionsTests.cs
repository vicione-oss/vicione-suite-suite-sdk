using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sdk.Backend.Extensions;
using Sdk.Backend.Modules;
using Sdk.Backend.Persistence;
using Sdk.Modules;
using TestModule.Backend;
using TestModule.Backend.DbContext;
using Xunit;

namespace Sdk.Backend.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public sealed class AddDynamicDbContext : ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _services = new();

        [Fact]
        public void Should_throw_if_module_initializer_is_null()
        {
            // Arrange
            var module = new TestBackendModule();

            // Act
            Action act = () => _services.AddDynamicDbContext<ITestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(module);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_db_context_base_is_no_interface()
        {
            // Arrange
            var module = new TestBackendModule();

            // Act
            Action act = () => _services.AddDynamicDbContext<TestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(module);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_register_required_services()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var workspaceService = Substitute.For<IWorkspaceProvider<TestBackendModule>>();
            var module = new TestBackendModule(initializer);

            _services.AddSingleton(workspaceService);

            // Act
            var provider = _services
                .AddDynamicDbContext<ITestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(module)
                .BuildServiceProvider();

            // Assert
            provider.GetService<DbContextResolverOptions<ITestModuleDbContext>>()
                .Should().NotBeNull();

            provider.GetService<DbContextResolver<TestModuleDbContextSqlite, TestModuleDbContextPostgres, ITestModuleDbContext>>()
                .Should().NotBeNull();

            provider.GetService<ITestModuleDbContext>()
                .Should().NotBeNull();
        }

        [Fact]
        public void Should_use_given_sqlite_db_name()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var workspaceService = Substitute.For<IWorkspaceProvider<TestBackendModule>>();
            var module = new TestBackendModule(initializer);
            const string SqliteDbName = "MySqliteDb.db";

            _services.AddSingleton(workspaceService);

            // Act
            var provider = _services
                .AddDynamicDbContext<ITestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(module, SqliteDbName)
                .BuildServiceProvider();

            // Assert
            var options = provider.GetRequiredService<DbContextResolverOptions<ITestModuleDbContext>>();
            options.DbName.Should().Be(SqliteDbName);
        }

        [Fact]
        public void Should_use_module_id_as_default_sqlite_db_name()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var workspaceService = Substitute.For<IWorkspaceProvider<TestBackendModule>>();
            var module = new TestBackendModule(initializer);

            _services.AddSingleton(workspaceService);

            // Act
            var provider = _services
                .AddDynamicDbContext<ITestModuleDbContext, TestModuleDbContextSqlite, TestModuleDbContextPostgres>(module)
                .BuildServiceProvider();

            // Assert
            var options = provider.GetRequiredService<DbContextResolverOptions<ITestModuleDbContext>>();
            options.DbName.Should().Be(module.ModuleId);
        }
    }

    public sealed class AddModuleSection : ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _serviceCollection = new();

        [Fact]
        public void Should_bind_section_options_for_module_id()
        {
            // Arrange
            var options = _serviceCollection.SetupTestOptions();

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(options);
        }

        [Fact]
        public void Should_bind_section_options_for_module()
        {
            // Arrange
            var options = _serviceCollection.SetupTestOptions();

            var module = Substitute.For<IModule>();
            module.ModuleKey.Returns(new ModuleKey
            {
                ModuleId = TestConstants.TestModuleId,
                ModuleType = ModuleType.Backend
            });

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(module);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(options);
        }

        [Fact]
        public void Should_bind_default_options_if_section_is_missing()
        {
            // Arrange
            _serviceCollection.SetupTestOptions("DoesNotExist");

            // Act
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Assert
            _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>()
                .Value.Should().BeEquivalentTo(new TestOptions());
        }

        [Fact]
        public void Should_validate_options_by_default()
        {
            // Arrange
            _serviceCollection.SetupInvalidTestOptions();
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId);

            // Act
            var action = () => _ = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>().Value;

            // Assert
            action.Should().Throw<OptionsValidationException>();
        }

        [Fact]
        public void Should_not_validate_options_if_disabled_by_param()
        {
            // Arrange
            _serviceCollection.SetupInvalidTestOptions();
            _serviceCollection.AddModuleSection<TestOptions>(TestConstants.TestModuleId, false);

            // Act
            var action = () => _ = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptions<TestOptions>>().Value;

            // Assert
            action.Should().NotThrow<OptionsValidationException>();
        }
    }
}
