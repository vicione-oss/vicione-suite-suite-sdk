using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Default implementation for notification elements with support for an icon, optional badge and flyout.
/// </summary>
/// <typeparam name="TState">Class implementing the state of the notification element</typeparam>
/// <typeparam name="TIcon">Class implementing the icon of the notification element</typeparam>
public abstract partial class NotificationElement<TState, TIcon> : NotificationElementBase<TState>, IDisposable
    where TState : class, INotificationElementState
    where TIcon : ComponentBase
{
    private readonly Type _iconComponentType = typeof(TIcon);
    private Type? _badgeComponentType;
    private Type? _flyoutComponentType;

    /// <summary>
    /// Override this method to provide a title for the notification element.
    /// The title will be displayed as tooltip when hovering the notification element.
    /// </summary>
    protected abstract string GetTitle();

    /// <summary>
    /// Call this method in constructor to register a badge.
    /// Once registered, it is displayed in the upper right corner of the notification element.
    /// </summary>
    protected void RegisterBadge<TBadge>() where TBadge : ComponentBase, INotificationElementBadge
        => _badgeComponentType = typeof(TBadge);

    /// <summary>
    /// Call this method in constructor to register a flyout.
    /// Once registered, it is rendered together with the notification element.
    /// Details like shape or when the flyout is displayed needs to be implemented by the flyout itself.
    /// </summary>
    protected void RegisterFlyout<TFlyout>() where TFlyout : ComponentBase, INotificationElementFlyout
        => _flyoutComponentType = typeof(TFlyout);

    /// <summary>
    /// Implements the click behavior of the notification element.
    /// By default, the flag <see cref="State.IsActive"/> is toggled.
    /// Override this method to implement custom click behavior.
    /// </summary>
    protected virtual void OnClick()
        => State.IsActive = !State.IsActive;

    [MustCallBase]
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        State.Changed -= OnStateChanged;
        State.Changed += OnStateChanged;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [MustCallBase]
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            State.Changed -= OnStateChanged;
    }

    [MustCallBase]
    protected virtual void OnStateChanged(NotificationElementStateChangedEventArgs args)
    {
        if (args.PropertyNames.Contains(nameof(State.IsActive)) || args.PropertyNames.Contains(nameof(State.Visible)))
            InvokeAsync(StateHasChanged);
    }
}
