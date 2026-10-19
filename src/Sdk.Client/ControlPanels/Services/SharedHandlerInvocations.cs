namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// The running invocations of the shared handlers of a scope, so that the control panels sharing a handler join one
/// invocation of it.
/// </summary>
internal sealed class SharedHandlerInvocations
{
    private readonly Lock _lock = new();
    private readonly Dictionary<(Type OwnerServiceKey, Type HandlerServiceType, Type HandlerType), Invocation> _runningInvocations = [];

    /// <summary>
    /// Invokes the shared handler, or joins its invocation that is running already.
    /// </summary>
    /// <param name="ownerServiceKey">The key of the control panel owning the entity, of which a scope edits one at a time.</param>
    /// <param name="handlerServiceType">
    /// The handler interface invoked, so a handler implementing several of them joins only invocations of the same one.
    /// </param>
    /// <param name="handlerType">The shared handler.</param>
    /// <param name="invoke">Invokes the handler when no invocation of it for the owner is running.</param>
    /// <param name="cancellationToken">
    /// Stops waiting for the invocation, which is canceled only once every caller joining it has stopped waiting, so one
    /// control panel ending its edit does not cancel it for the others.
    /// </param>
    public async Task<TResult> InvokeOrJoin<TResult>(Type ownerServiceKey, Type handlerServiceType, Type handlerType,
        Func<CancellationToken, Task<TResult>> invoke, CancellationToken cancellationToken)
    {
        var key = (ownerServiceKey, handlerServiceType, handlerType);

        Invocation? invocation;

        lock (_lock)
        {
            if (!_runningInvocations.TryGetValue(key, out invocation))
            {
                invocation = new Invocation();
                invocation.Task = InvokeAndRemove(key, invocation, invoke);

                if (!invocation.Task.IsCompleted)
                    _runningInvocations[key] = invocation;
            }

            invocation.Callers++;
        }

        try
        {
            return await ((Task<TResult>)invocation.Task).WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Leave(key, invocation);

            throw;
        }
    }

    private async Task<TResult> InvokeAndRemove<TResult>((Type, Type, Type) key, Invocation invocation,
        Func<CancellationToken, Task<TResult>> invoke)
    {
        try
        {
            return await invoke(invocation.CancellationTokenSource.Token);
        }
        finally
        {
            lock (_lock)
            {
                invocation.Ended = true;
                RemoveRunning(key, invocation);
            }

            invocation.CancellationTokenSource.Dispose();
        }
    }

    private void Leave((Type, Type, Type) key, Invocation invocation)
    {
        lock (_lock)
        {
            if (--invocation.Callers > 0 || invocation.Ended)
                return;

            // A caller arriving from now on starts a new invocation rather than joining the canceled one.
            RemoveRunning(key, invocation);
        }

        try
        {
            invocation.CancellationTokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // The invocation ended between leaving the lock and canceling it, so there is nothing left to cancel.
        }

        // No caller awaits the invocation any more, so a failure after the cancellation would go unobserved.
        _ = invocation.Task.ContinueWith(static task => _ = task.Exception, CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    private void RemoveRunning((Type, Type, Type) key, Invocation invocation)
    {
        if (_runningInvocations.TryGetValue(key, out var running) && ReferenceEquals(running, invocation))
            _runningInvocations.Remove(key);
    }

    private sealed class Invocation
    {
        public CancellationTokenSource CancellationTokenSource { get; } = new();

        public Task Task { get; set; } = Task.CompletedTask;

        /// <summary>
        /// Gets or sets the number of callers that joined the invocation and have not stopped waiting for it.
        /// </summary>
        public int Callers { get; set; }

        public bool Ended { get; set; }
    }
}
