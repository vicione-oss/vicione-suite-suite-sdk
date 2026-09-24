using Sdk.Client.Wizards.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.Wizards.Components;

/// <summary>
/// Base class for wizard pages.
/// </summary>
public partial class WizardPage<TState> : ComponentBase, IWizardPage<TState>, IAsyncDisposable
    where TState : IWizardPageState
{
    private bool _disposedAsync;

    /// <summary>
    /// Gets or sets the page's state, which keeps its values across render cycles.
    /// </summary>
    [Parameter, EditorRequired] public TState State { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked by <see cref="BeginEdit"/>.
    /// </summary>
    [Parameter] public EventCallback OnBeginEdit { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked by <see cref="CancelEdit"/>.
    /// </summary>
    [Parameter] public EventCallback OnCancelEdit { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked after every render of the page.
    /// </summary>
    [Parameter] public EventCallback<WizardPageAfterRenderCycleEventArgs> OnAfterRenderCycle { get; set; }

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
    /// </summary>
    /// <remarks>
    /// This method is called from within <see cref="DisposeAsync"/>.
    /// </remarks>
    [MustCallBase]
    protected virtual ValueTask DisposeAsyncCore()
        => ValueTask.CompletedTask;

    /// <inheritdoc/>
    [MustCallBase]
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (OnAfterRenderCycle.HasDelegate)
            await OnAfterRenderCycle.InvokeAsync(new WizardPageAfterRenderCycleEventArgs(firstRender));
    }

    /// <summary>
    /// Signals that an edit on the page has begun.
    /// </summary>
    /// <remarks>
    /// This method invokes the <see cref="OnBeginEdit"/> callback.
    /// </remarks>
    [MustCallBase]
    protected virtual async Task BeginEdit()
    {
        if (OnBeginEdit.HasDelegate)
            await OnBeginEdit.InvokeAsync();
    }

    /// <summary>
    /// Signals a request for cancellation of an already running edit.
    /// </summary>
    /// <remarks>
    /// This method invokes the <see cref="OnCancelEdit"/> callback.
    /// </remarks>
    [MustCallBase]
    protected virtual async Task CancelEdit()
    {
        if (OnCancelEdit.HasDelegate)
            await OnCancelEdit.InvokeAsync();
    }
}
