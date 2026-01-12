using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Provides a default implementation for a notification element's flyout panel with support for
/// customizable content and an optional heading / close button.
/// </summary>
/// <remarks>
/// The flyout will be displayed when the notification element is <see cref="NotificationElementState.IsActive">active</see>.
/// </remarks>
/// <typeparam name="TState">The type of the state object for the associated notification element.</typeparam>
/// <typeparam name="TContent">The type of the component that renders the flyout's content.</typeparam>
public partial class NotificationElementFlyout<TState, TContent> : ComponentBase, INotificationElementFlyout, IDisposable
    where TState : INotificationElementState
    where TContent : ComponentBase, INotificationElementFlyoutContent
{
    private readonly Type _contentComponentType = typeof(TContent);

    /// <summary>
    /// Gets or sets the state of the associated notification element, provided as a cascading parameter.
    /// </summary>
    [CascadingParameter]
    public required TState NotificationElementState { get; set; }

    /// <summary>
    /// Gets the text to be displayed in the flyout's header.
    /// </summary>
    /// <returns>The heading text, or <c>null</c> to hide the header and the close button.</returns>
    protected virtual string? GetHeading() => null;

    /// <inheritdoc/>
    [MustCallBase]
    protected override void OnInitialized()
        => NotificationElementState.Changed += OnNotificationElementStateChanged;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Provides a hook for derived classes to perform their own disposal logic.
    /// </summary>
    /// <remarks>
    /// This method is called from within <see cref="Dispose()"/>.
    /// </remarks>
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
