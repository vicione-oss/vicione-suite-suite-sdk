using AwesomeAssertions;
using Sdk.Backend.Modules;
using Sdk.Modules;
using TestModule.Backend;
using Xunit;

namespace Sdk.Backend.Tests.Modules;

public class BackendModuleTests
{
    [Fact]
    public void Should_resolve_module_id_by_generic_type()
    {
        // Arrange
        var moduleId = ModuleIdResolver.ResolveId<TestBackendModule>();

        // Act
        var module = new TestBackendModule();

        // Assert
        module.ModuleId.Should().Be(moduleId);
        module.ModuleKey.ModuleType.Should().Be(ModuleType.Backend);
    }

    [Fact]
    public void Should_resolve_module_id_by_assembly()
    {
        // Arrange
        var moduleId = ModuleIdResolver.ResolveId(typeof(TestBackendModule).Assembly);

        // Act
        var module = new TestBackendModule();

        // Assert
        module.ModuleId.Should().Be(moduleId);
    }

    [Theory]
    [InlineData("ViciOne.Suite.Module.Backend", "ViciOne.Suite.Module")]
    [InlineData("ViciOne.Suite.Module.Client", "ViciOne.Suite.Module")]
    [InlineData("ViciOne.Suite.Module.Internal", "ViciOne.Suite.Module")]
    [InlineData("ViciOne.Suite.Module.Public", "ViciOne.Suite.Module")]

    public void Should_resolve_module_id_by_assembly_name(string assemblyName, string expectedId)
    {
        // Arrange + Act
        var moduleId = ModuleIdResolver.ResolveId(assemblyName);

        // Assert
        moduleId.Should().Be(expectedId);
    }

    [Fact]
    public void Should_resolve_module_id()
    {
        // Arrange
        var moduleId = ModuleIdResolver.ResolveId<TestBackendModule>();

        // Act
        var module = new TestBackendModule();

        // Assert
        module.ModuleId.Should().Be(moduleId);
        module.ModuleKey.ModuleType.Should().Be(ModuleType.Backend);
    }

    [Fact]
    public void Should_throw_if_resolve_fails()
    {
        // Arrange
        var module = new LocalTestBackendModule();

        // Act
        var action = () => _ = module.ModuleId;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData("ViciOne.Suite.Module")]
    [InlineData("ViciOne.Suite.Module.Clien")]
    [InlineData("ViciOne.Suite.Module.Internalt")]
    [InlineData("ViciOne.Suite.Module.Shared")]
    public void Should_throw_if_resolve_naming_convention_is_violated(string assemblyName)
    {
        // Arrange + Act
        var action = () => _ = ModuleIdResolver.ResolveId(assemblyName);

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    private sealed class LocalTestBackendModule : BackendModule;
}
