using Castle.Core.Internal;
using AwesomeAssertions;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Attributes;
using Sdk.Client.Tests.NotificationArea.Extensions;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea;

public sealed class InitialNotificationElementAttributeTests
{
    [Fact]
    public void Should_hold_random_id()
    {
        // Arrange
        var attribute = new InitialNotificationElementAttribute<TestClientModule>();

        // Assert
        attribute.Id.Should().RepresentGuid();
    }

    [Fact]
    public void Should_hold_specified_id()
    {
        // Arrange
        const string Id = "2752fc69-f4da-4b28-8e2b-2aef66e4f24c";

        var attribute = new InitialNotificationElementAttribute<TestClientModule> { Id = Id, };

        // Assert
        attribute.Id.Should().Be(Id);
    }

    [Fact]
    public void Should_decorate_classes_only()
    {
        // Arrange
        var attributeUsage = typeof(InitialNotificationElementAttribute<TestClientModule>).GetAttributeUsage();

        // Assert
        attributeUsage.ValidOn.Should().HaveFlag(AttributeTargets.Class);
    }

    [Fact]
    public void Should_decorate_a_class_only_once()
    {
        // Arrange
        var attributeUsage = typeof(InitialNotificationElementAttribute<TestClientModule>).GetAttributeUsage();

        // Assert
        attributeUsage.AllowMultiple.Should().BeFalse();
    }

    private sealed class TestClientModule : ClientModule;
}
