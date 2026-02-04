using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.MaterialDesignIcon;
using Sdk.Client.Enums;
using Sdk.Client.Extensions;
using Xunit;

namespace Sdk.Client.Tests.Components.MaterialDesignIcon;

public sealed class MaterialDesignIconComponentTests
{
    public static readonly TheoryData<string> IconNames = [.. Enum.GetNames<MaterialDesignIconName>()];

    [Fact]
    public void Should_render_component()
    {
        // Arrange
        using var ctx = new BunitContext();

        // Act
        var renderedComponent = ctx.Render<MaterialDesignIconComponent>();

        // Assert
        renderedComponent.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(IconNames))]
    public void Should_render_icon(string iconName)
    {
        // Arrange
        var iconNameTyped = Enum.Parse<MaterialDesignIconName>(iconName);
        using var ctx = new BunitContext();

        // Act
        var renderedComponent = ctx.Render<MaterialDesignIconComponent>((p) => p.Add(c => c.Name, iconNameTyped));

        // Assert
        var icon = renderedComponent.Find("i");
        icon.ClassList.Should().Contain("mdi");
        icon.ClassList.Should().Contain("mdi-24px");
        icon.ClassList.Should().Contain($"mdi-{iconNameTyped.ToString().ToHyphenSeparated()}");
    }
}
