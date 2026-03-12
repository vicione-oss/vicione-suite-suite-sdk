using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldSwitchTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();
        ctx.SetupSuiteServices();

        // Act
        var component = ctx.Render<SettingsFieldSwitch>(b =>
            b.Add(p => p.Value, true));

        // Assert
        component.Should().NotBeNull();
    }
}
