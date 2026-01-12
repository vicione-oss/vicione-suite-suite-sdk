using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Modules;
using Sdk.Client.Modules.Localization;
using Sdk.Client.Modules.Localization.Extensions;
using Sdk.Modules;
using Xunit;

namespace Sdk.Client.Tests.Modules.Localization;

public sealed class ClientModuleLocalizerTests
{
    [Fact]
    public void Should_be_resolvable_per_module()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLocalization<TestClientModuleA, Localizer<TestClientModuleA>>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var clientModuleLocalizer = serviceProvider.GetService<IClientModuleLocalizer<TestClientModuleA>>();

        // Assert
        clientModuleLocalizer.Should().NotBeNull();
    }

    [Fact]
    public void Should_be_resolvable_for_all_modules()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLocalization<TestClientModuleA, Localizer<TestClientModuleA>>()
            .AddLocalization<TestClientModuleB, Localizer<TestClientModuleB>>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var clientModuleLocalizers = serviceProvider.GetRequiredService<IEnumerable<IClientModuleLocalizer<IClientModule>>>().ToList();

        // Assert
        clientModuleLocalizers.Should().NotBeNull().And.HaveCount(2);
        clientModuleLocalizers.First().Should().BeAssignableTo<IClientModuleLocalizer<TestClientModuleA>>();
        clientModuleLocalizers.Skip(1).First().Should().BeAssignableTo<IClientModuleLocalizer<TestClientModuleB>>();
    }

    private sealed class TestClientModuleA : IClientModule
    {
        public ModuleKey ModuleKey => new();

        public IEnumerable<ModuleKey> Dependencies => [];
    }

    private sealed class TestClientModuleB : IClientModule
    {
        public ModuleKey ModuleKey => new();

        public IEnumerable<ModuleKey> Dependencies => [];
    }

    private sealed class Localizer<TClientModule> : IClientModuleLocalizer<TClientModule>
        where TClientModule : IClientModule
    {
        public string GetTitle() => "Test client module";
    }
}
