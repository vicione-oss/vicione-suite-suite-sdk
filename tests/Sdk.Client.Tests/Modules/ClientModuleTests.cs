using AwesomeAssertions;
using Sdk.Client.Modules;
using Sdk.Modules;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Modules;

public class ClientModuleTests
{
    [Fact]
    public void Should_resolve_module_id()
    {
        // Arrange
        var moduleId = ModuleIdResolver.ResolveId<TestClientModule>();

        // Act
        var module = new TestClientModule();

        // Assert
        module.ModuleId.Should().Be(moduleId);
        module.ModuleKey.ModuleType.Should().Be(ModuleType.Client);
    }

    [Fact]
    public void Should_throw_if_module_id_resolve_fails()
    {
        // Arrange
        var module = new LocalTestClientModule();

        // Act
        var action = () => _ = module.ModuleId;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    private sealed class LocalTestClientModule : ClientModule;
}
