using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Default implementation of a notification element flyout with support for customizable content and an optional heading / close button.
/// 
/// The flyout will be displayed when the notification element is <see cref="NotificationElementState.IsActive">active</see>.
/// </summary>
/// <typeparam name="TState">Class implementing the state of the associated notification element</typeparam>
/// <typeparam name="TContent">Class implementing the flyout content</typeparam>
public partial class NotificationElementFlyout<TState, TContent> : ComponentBase, INotificationElementFlyout, IDisposable
    where TState : INotificationElementState
    where TContent : ComponentBase, INotificationElementFlyoutContent
{
    private readonly Type _contentComponentType = typeof(TContent);

    [CascadingParameter]
    public required TState NotificationElementState { get; set; }

    /// <returns>Text displayed in the heading area or null to hide the heading area together with the associated close button</returns>
    protected virtual string? GetHeading() => null;

    [MustCallBase]
    protected override void OnInitialized()
        => NotificationElementState.Changed += OnNotificationElementStateChanged;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [MustCallBase]
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            NotificationElementState.Changed -= OnNotificationElementStateChanged;
    }

    private void OnNotificationElementStateChanged(NotificationElementStateChangedEventArgs args)
    {
        if (args.PropertyNames.Contains(nameof(NotificationElementState.IsActive)))
            InvokeAsync(StateHasChanged);
    }
}
