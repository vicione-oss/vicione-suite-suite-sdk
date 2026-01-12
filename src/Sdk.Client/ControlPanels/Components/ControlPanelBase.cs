using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Client.ControlPanels.Components;

public abstract partial class ControlPanelBase<TState> : ComponentBase, IAsyncDisposable, IControlPanel
    where TState : IControlPanelState
{
    private bool _disposedAsync;
    private IControlPanelSaveHandler<TState>? _saveHandler;
    private IControlPanelCancelHandler<TState>? _cancelHandler;
    private IControlPanelResetHandler<TState>? _resetHandler;
    private CancellationTokenSource? _handlerMethodInvocationCancellationTokenSource;
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly CancellationTokenSource _semaphoreCancellationTokenSource = new();

    [Parameter, EditorRequired] public TState State { get; set; }

    [Inject] private IControlPanelService ControlPanelService { get; set; } = default!;

    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject] private ILogger<ControlPanelBase<TState>> Logger { get; set; } = default!;

    protected bool IsDirty => ControlPanelService.IsDirty;

    [MustCallBase]
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (_disposedAsync)
            return;

        _resetHandler ??= ServiceProvider.GetService<IControlPanelResetHandler<TState>>();

        if (_resetHandler is not null)
        {
            try
            {
                var cancellationToken = await GetCancellationTokenForHandlerMethodInvocation(_semaphoreCancellationTokenSource.Token);

                await _resetHandler.Reset(State, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // nothing to do here, we just return gracefully

                return;
            }
        }

        ControlPanelService.OnSave -= ControlPanelServiceOnSave;
        ControlPanelService.OnSave += ControlPanelServiceOnSave;

        ControlPanelService.OnCancel -= ControlPanelServiceOnCancel;
        ControlPanelService.OnCancel += ControlPanelServiceOnCancel;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposedAsync, true, false))
            return;

        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [MustCallBase]
    protected virtual async ValueTask DisposeAsyncCore()
    {
        // For now, we do not call CancelEdit because it would require an await on the async behavior
        // which could result in memory leaks when the await never returns

        ControlPanelService.OnCancel -= ControlPanelServiceOnCancel;
        ControlPanelService.OnSave -= ControlPanelServiceOnSave;

        await _semaphoreCancellationTokenSource.CancelAsync();
        _semaphoreCancellationTokenSource.Dispose();

        await _semaphore.WaitAsync();
        try
        {
            if (_handlerMethodInvocationCancellationTokenSource is not null)
            {
                try
                {
                    await _handlerMethodInvocationCancellationTokenSource.CancelAsync();
                }
                catch (Exception e)
                {
                    UnexpectedErrorWhileRequestingCancellation(Logger, e);
                }

                _handlerMethodInvocationCancellationTokenSource.Dispose();
            }
        }
        finally
        {
            _semaphore.Release();
        }

        _semaphore.Dispose();
    }

    private async Task<ISaveResult> ControlPanelServiceOnSave()
    {
        if (_disposedAsync)
            return new SaveSuccessResult();

        _saveHandler ??= ServiceProvider.GetService<IControlPanelSaveHandler<TState>>();

        if (_saveHandler == null)
            return new SaveErrorResult("No save handler found.");

        try
        {
            var cancellationToken = await GetCancellationTokenForHandlerMethodInvocation(_semaphoreCancellationTokenSource.Token);

            var saveResult = await _saveHandler.Save(State, cancellationToken);

            return saveResult;
        }
        catch (OperationCanceledException)
        {
            // nothing to do here, we just return gracefully
            return new SaveSuccessResult();
        }
    }

    private async Task ControlPanelServiceOnCancel()
    {
        if (_disposedAsync)
            return;

        _cancelHandler ??= ServiceProvider.GetService<IControlPanelCancelHandler<TState>>();

        if (_cancelHandler is not null)
        {
            try
            {
                var cancellationToken = await GetCancellationTokenForHandlerMethodInvocation(_semaphoreCancellationTokenSource.Token);

                await _cancelHandler.Cancel(State, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // nothing to do here, we just return gracefully

                return;
            }
        }

        if (_resetHandler is null)
            return;

        try
        {
            var cancellationToken = await GetCancellationTokenForHandlerMethodInvocation(_semaphoreCancellationTokenSource.Token);

            await _resetHandler.Reset(State, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // nothing to do here, we just return gracefully
        }
    }

    /// <summary>
    /// Invoked by the control panel, when and edit takes place
    /// </summary>
    protected virtual Task OnEdit()
        => Task.CompletedTask;

    /// <summary>
    /// Call this to set the control panel to edit mode.
    /// </summary>
    protected async Task BeginEdit()
    {
        await OnEdit();

        await ControlPanelService.BeginEdit();
    }

    /// <summary>
    /// Call this to cancel the edit mode for the control panel.
    /// </summary>
    protected Task CancelEdit()
        => ControlPanelService.CancelEdit();

    private async Task<CancellationToken> GetCancellationTokenForHandlerMethodInvocation(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_handlerMethodInvocationCancellationTokenSource is not null)
            {
                await _handlerMethodInvocationCancellationTokenSource.CancelAsync();

                _handlerMethodInvocationCancellationTokenSource.Dispose();
            }

            _handlerMethodInvocationCancellationTokenSource = new CancellationTokenSource();

            return _handlerMethodInvocationCancellationTokenSource.Token;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    [LoggerMessage(1, LogLevel.Warning, "Unexpected error while requesting cancellation of possible ongoing handler execution")]
    internal static partial void UnexpectedErrorWhileRequestingCancellation(ILogger<ControlPanelBase<TState>> logger, Exception exception);
}
