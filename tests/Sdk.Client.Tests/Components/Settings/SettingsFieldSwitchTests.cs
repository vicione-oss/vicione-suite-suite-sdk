using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using ViciOne.Ui.Blazor.Components.Switch;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldSwitchTests
{
    [Fact]
    public async Task Should_render_component()
    {
        // Arrange
        await using var ctx = new BunitContext();

        // Act
        var component = ctx.Render<SettingsFieldSwitch>(b =>
            b.Add(p => p.Value, true));

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_propagate_exception_of_value_changed_handler()
    {
        // Arrange
        await using var ctx = new BunitContext();

        var component = ctx.Render<SettingsFieldSwitch>(b =>
            b.Add(p => p.ValueChanged, _ => Task.FromException(new InvalidOperationException("Handler failed."))));

        var switchComponent = component.FindComponent<Switch>();

        // Act
        var act = () => switchComponent.InvokeAsync(() => switchComponent.Instance.ValueChanged.InvokeAsync(true));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Handler failed.");
    }
}
