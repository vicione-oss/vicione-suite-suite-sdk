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

    public sealed class AddModuleDbContext : ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _serviceCollection = new();

        [Fact]
        public void Should_throw_if_module_has_no_initializer()
        {
            // Arrange
            var module = new TestBackendModule(); // no initializer
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(new FakeRegistrar());

            // Act
            var action = () =>
                _serviceCollection
                    .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                        TestModuleDbContextPostgres>(module);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*no initializer*");
        }

        [Fact]
        public void Should_throw_if_no_registrar_is_registered()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);

            // Act
            var action = () =>
                _serviceCollection
                    .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                        TestModuleDbContextPostgres>(module);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*IModuleDbContextRegistrar*");
        }

        [Fact]
        public void Should_find_registrar_registered_as_instance()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var registrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(registrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);

            // Assert
            registrar.RegisterCalled.Should().BeTrue();
            registrar.LastModuleId.Should().Be(module.ModuleId);
        }

        [Fact]
        public void Should_find_registrar_registered_by_type()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar, FakeRegistrar>();

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);

            // Assert — the type registration should have been resolved and re-pinned as an instance
            var descriptor = _serviceCollection.Last(d => d.ServiceType == typeof(IModuleDbContextRegistrar));
            descriptor.ImplementationInstance.Should().NotBeNull()
                .And.BeOfType<FakeRegistrar>();
            ((FakeRegistrar)descriptor.ImplementationInstance!).RegisterCalled.Should().BeTrue();
        }

        [Fact]
        public void Should_find_registrar_registered_by_factory()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var registrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(_ => registrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);

            // Assert
            registrar.RegisterCalled.Should().BeTrue();
            var descriptor = _serviceCollection.Last(d => d.ServiceType == typeof(IModuleDbContextRegistrar));
            descriptor.ImplementationInstance.Should().BeSameAs(registrar);
        }

        [Fact]
        public void Should_reuse_same_registrar_instance_across_multiple_calls()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar, FakeRegistrar>();

            // Act — call twice
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);
            _serviceCollection
                .AddModuleDbContext<IReferenceDbContext, ReferenceDbContextSqlite,
                    ReferenceDbContextPostgres>(module);

            // Assert — only one registrar descriptor should remain and both calls used the same instance
            var descriptors = _serviceCollection
                .Where(d => d.ServiceType == typeof(IModuleDbContextRegistrar))
                .ToList();
            descriptors.Should().HaveCount(1);

            var registrar = (FakeRegistrar)descriptors[0].ImplementationInstance!;
            registrar.CallCount.Should().Be(2);
        }

        [Fact]
        public void Should_use_module_id_as_default_sqlite_db_name()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var registrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(registrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);

            // Assert
            registrar.LastSqliteDbName.Should().Be(module.ModuleId);
        }

        [Fact]
        public void Should_use_custom_sqlite_db_name_when_provided()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var registrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(registrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module, sqliteDbName: "CustomDb");

            // Assert
            registrar.LastSqliteDbName.Should().Be("CustomDb");
        }

        [Fact]
        public void Should_pass_enable_synchronization_flag()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var registrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(registrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module, enableSynchronization: false);

            // Assert
            registrar.LastEnableSynchronization.Should().BeFalse();
        }

        [Fact]
        public void Should_use_last_registered_registrar()
        {
            // Arrange
            var initializer = Substitute.For<IModuleInitializer>();
            var module = new TestBackendModule(initializer);
            var firstRegistrar = new FakeRegistrar();
            var secondRegistrar = new FakeRegistrar();
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(firstRegistrar);
            _serviceCollection.AddSingleton<IModuleDbContextRegistrar>(secondRegistrar);

            // Act
            _serviceCollection
                .AddModuleDbContext<ITestModuleDbContext, TestModuleDbContextSqlite,
                    TestModuleDbContextPostgres>(module);

            // Assert
            firstRegistrar.RegisterCalled.Should().BeFalse();
            secondRegistrar.RegisterCalled.Should().BeTrue();
        }
    }
}

/// <summary>
/// A fake <see cref="IModuleDbContextRegistrar"/> that records calls for assertion.
/// </summary>
file sealed class FakeRegistrar : IModuleDbContextRegistrar
{
    public bool RegisterCalled { get; private set; }
    public int CallCount { get; private set; }
    public string? LastModuleId { get; private set; }
    public string? LastSqliteDbName { get; private set; }
    public bool LastEnableSynchronization { get; private set; } = true;

    public void Register<TDbContextInterface, TSqliteImplementation, TPostgresImplementation>(
        IServiceCollection services,
        string moduleId,
        Type moduleType,
        string sqliteDbName,
        bool enableSynchronization)
        where TDbContextInterface : IModuleDbContext
        where TSqliteImplementation : Microsoft.EntityFrameworkCore.DbContext, ISqliteDbContext, TDbContextInterface
        where TPostgresImplementation : Microsoft.EntityFrameworkCore.DbContext, IPostgresDbContext, TDbContextInterface
    {
        RegisterCalled = true;
        CallCount++;
        LastModuleId = moduleId;
        LastSqliteDbName = sqliteDbName;
        LastEnableSynchronization = enableSynchronization;
    }
}
