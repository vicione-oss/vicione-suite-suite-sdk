using Microsoft.AspNetCore.Components;
using Sdk.Client.ControlPanels.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.ControlPanels.Components;

/// <summary>
/// An abstract base class for control panel components.
/// </summary>
public abstract partial class ControlPanelBase<TState> : ComponentBase, IAsyncDisposable, IControlPanel
    where TState : IControlPanelState
{
    private bool _disposedAsync;

    /// <summary>
    /// Gets or sets the state object for the control panel, which holds data independently of the component's render cycle.
    /// </summary>
    [Parameter, EditorRequired] public TState State { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the panel enters an edit mode via a call to <see cref="BeginEdit"/>.
    /// </summary>
    [Parameter] public EventCallback OnBeginEdit { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the edit mode is cancelled via a call to <see cref="CancelEdit"/>.
    /// </summary>
    [Parameter] public EventCallback OnCancelEdit { get; set; }

    /// <summary>
    /// Gets a value indicating whether the control panel has unsaved changes.
    /// </summary>
    [Obsolete("Implement own dirty state handling, e.g. by subscribing to State.Changed")]
    protected bool IsDirty => false;

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposedAsync, true, false))
            return;

        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Provides a hook for derived classes to perform their own asynchronous disposal logic.
    /// This method is called from within <see cref="DisposeAsync"/>.
    /// </summary>
    [MustCallBase]
    protected virtual ValueTask DisposeAsyncCore()
        => ValueTask.CompletedTask;

    /// <summary>
    /// Invoked by the control panel, when and edit takes place.
    /// </summary>
    [Obsolete("Override " + nameof(BeginEdit) + "() instead")]
    protected virtual Task OnEdit()
        => Task.CompletedTask;

    /// <summary>
    /// Call this to indicate that edit has begun.
    /// </summary>
    [MustCallBase]
    protected virtual async Task BeginEdit()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await OnEdit();
#pragma warning restore CS0618 // Type or member is obsolete

        if (OnBeginEdit.HasDelegate)
            await OnBeginEdit.InvokeAsync();
    }

    /// <summary>
    /// Call this to request cancellation of an already running edit.
    /// </summary>
    [MustCallBase]
    protected virtual async Task CancelEdit()
    {
        if (OnCancelEdit.HasDelegate)
            await OnCancelEdit.InvokeAsync();
    }
}
