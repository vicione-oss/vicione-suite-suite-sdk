using Bunit;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;

namespace Sdk.Client.Tests.NotificationArea.Assertions;

internal sealed class NotificationElementAssertions<TNotificationElement>(IRenderedComponent<TNotificationElement> subject)
    : ReferenceTypeAssertions<IRenderedComponent<TNotificationElement>, NotificationElementAssertions<TNotificationElement>>(subject, AssertionChain.GetOrCreate())
    where TNotificationElement : ComponentBase, INotificationElement
{
    protected override string Identifier => Subject.GetType().Name;

    public AndConstraint<NotificationElementAssertions<TNotificationElement>> BeActive(string because = "",
        params object[] becauseArgs)
    {
        var notificationElement = Subject.Find(".notification-element");
        var isActive = notificationElement.ClassList.Contains("notification-element--active");

        CurrentAssertionChain
            .ForCondition(isActive)
            .BecauseOf(because, becauseArgs)
            .WithDefaultIdentifier(Identifier)
            .FailWith("Expected {context} to be {0}{reason}, but found {1}.", true, isActive);

        return new AndConstraint<NotificationElementAssertions<TNotificationElement>>(this);
    }
}
