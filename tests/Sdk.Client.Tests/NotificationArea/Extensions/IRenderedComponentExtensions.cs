using Bunit;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.Tests.NotificationArea.Assertions;

namespace Sdk.Client.Tests.NotificationArea.Extensions;

internal static class IRenderedComponentExtensions
{
    public static NotificationElementAssertions<T> Should<T>(this IRenderedComponent<T> renderedComponent)
        where T : ComponentBase, INotificationElement
            => new(renderedComponent);
}
