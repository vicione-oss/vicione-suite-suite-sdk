using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Provides a default implementation for a notification element with support for an icon, optional badge and flyout.
/// </summary>
/// <typeparam name="TState">The type of the state object for the notification element.</typeparam>
/// <typeparam name="TIcon">The type of the component that renders the element's icon.</typeparam>
public abstract partial class NotificationElement<TState, TIcon> : NotificationElementBase<TState>, IDisposable
    where TState : class, INotificationElementState
    where TIcon : ComponentBase
{
    private readonly Type _iconComponentType = typeof(TIcon);
    private Type? _badgeComponentType;
    private Type? _flyoutComponentType;

    /// <summary>
    /// Gets the title for the notification element.
    /// The title will be displayed as tooltip when hovering the notification element
    /// </summary>
    protected abstract string GetTitle();

    /// <summary>
    /// Registers a badge to be displayed in the upper-right corner of the notification element.
    /// </summary>
    /// <remarks>
    /// This method must be called from the constructor of the derived class.
    /// </remarks>
    protected void RegisterBadge<TBadge>() where TBadge : ComponentBase, INotificationElementBadge
        => _badgeComponentType = typeof(TBadge);

    /// <summary>
    /// Registers a flyout component to be rendered when the notification element is active.
    /// </summary>
    /// <remarks>
    /// This method must be called from the constructor of the derived class.
    /// </remarks>
    protected void RegisterFlyout<TFlyout>() where TFlyout : ComponentBase, INotificationElementFlyout
        => _flyoutComponentType = typeof(TFlyout);

    /// <summary>
    /// Handles the click behavior for the notification element.
    /// </summary>
    /// <remarks>
    /// The default implementation toggles the <see cref="INotificationElementState.IsActive"/> property.
    /// Override this method to implement custom click behavior.
    /// </remarks>
    protected virtual void OnClick()
        => State.IsActive = !State.IsActive;

    /// <inheritdoc/>
    [MustCallBase]
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        State.Changed -= OnStateChanged;
        State.Changed += OnStateChanged;
    }

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
            State.Changed -= OnStateChanged;
    }

    /// <summary>
    /// Handles notifications when properties of the state object have changed.
    /// Also, provides a hook for derived classes to perform their own state change logic.
    /// </summary>
    [MustCallBase]
    protected virtual void OnStateChanged(NotificationElementStateChangedEventArgs args)
    {
        if (args.PropertyNames.Contains(nameof(State.IsActive)) || args.PropertyNames.Contains(nameof(State.Visible)))
            InvokeAsync(StateHasChanged);
    }
}
