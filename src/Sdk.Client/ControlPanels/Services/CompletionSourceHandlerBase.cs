using System.Collections.Concurrent;
using MassTransit;
using Sdk.Client.Infrastructure;
using Sdk.Messaging;
using Sdk.Utils;
using ViciOne.Ui.Localization.Resources;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Abstract base class that implements the fire-and-correlate messaging pattern:
/// a command is sent to the backend via <see cref="IUiMediator"/>, and the caller
/// awaits a correlated backend event that resolves the operation within a
/// <see cref="IUiMediator.CommandTimeoutMs"/> timeout window.
/// </summary>
/// <remarks>
/// <para>
/// Concrete subclasses register as <see cref="IEventConsumer{TEvent}"/> for the
/// expected response events and call <see cref="CompleteWithSuccess"/> or
/// <see cref="CompleteWithError"/> from their <c>Consume</c> implementations,
/// using the event's <c>CorrelationId</c> to match the pending operation.
/// </para>
/// <para>
/// Event subscriptions must be registered in the subclass constructor via
/// <see cref="Register{TEvent}"/> and are automatically cleaned up on disposal.
/// </para>
/// </remarks>
/// <typeparam name="TServiceResult">
/// The result type returned to the caller after command completion or failure.
/// </typeparam>
public abstract class CompletionSourceHandlerBase<TServiceResult>(IUiMediator uiMediator) : IDisposable
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ErrorInfo?>> _taskCompletionSourceMap = new();
    private readonly AutoDisposeList<IDisposable> _subscriptions = [];
    private volatile bool _disposed;

    /// <summary>
    /// Gets the mediator used to send commands and register event consumers.
    /// </summary>
    protected IUiMediator Mediator { get; } = uiMediator;

    /// <summary>
    /// Registers the given consumer of <typeparamref name="TEvent"/>
    /// events and tracks the subscription for automatic disposal.
    /// </summary>
    /// <typeparam name="TEvent">The event type to consume.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if the current instance does not implement <see cref="IEventConsumer{TEvent}"/>.</exception>"
    protected void Register<TEvent>() where TEvent : class, IEvent
    {
        if (this is not IEventConsumer<TEvent> handler)
            throw new InvalidOperationException($"The current instance does not implement IEventConsumer<{typeof(TEvent).Name}> and cannot register for {typeof(TEvent).Name} events.");

        _subscriptions.Add(Mediator.Register(handler));
    }

    /// <summary>
    /// Resolves the pending operation identified by <paramref name="correlationId"/> as successful.
    /// Call this from a <c>Consume</c> implementation when the backend signals completion without error.
    /// </summary>
    /// <param name="correlationId">The correlation ID from the received event.</param>
    /// <returns><see langword="true"/> if a pending operation was found and completed; otherwise <see langword="false"/>.</returns>
    protected bool CompleteWithSuccess(Guid correlationId)
    {
        if (_taskCompletionSourceMap.TryRemove(correlationId, out var taskCompletionSource))
        {
            taskCompletionSource.SetResult(null);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resolves the pending operation identified by <paramref name="correlationId"/> with the supplied error.
    /// Call this from a <c>Consume</c> implementation when the backend signals a failure.
    /// </summary>
    /// <param name="correlationId">The correlation ID from the received event.</param>
    /// <param name="errorInfo">Error details from the backend, or <see langword="null"/> for a generic failure.</param>
    /// <returns><see langword="true"/> if a pending operation was found and completed; otherwise <see langword="false"/>.</returns>
    protected bool CompleteWithError(Guid correlationId, ErrorInfo? errorInfo)
    {
        if (_taskCompletionSourceMap.TryRemove(correlationId, out var taskCompletionSource))
        {
            taskCompletionSource.SetResult(errorInfo);
            return true;
        }

        return false;
    }

    /// <summary>Finalizer — invokes <see cref="Dispose(bool)"/> for unmanaged resource cleanup.</summary>
    ~CompletionSourceHandlerBase() => Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources held by this instance.
    /// </summary>
    /// <remarks>
    /// When <paramref name="disposing"/> is <see langword="true"/>, all active event subscriptions
    /// are disposed and any pending operations are cancelled. Derived classes should override this
    /// method to release their own resources, calling <c>base.Dispose(disposing)</c> last.
    /// </remarks>
    /// <param name="disposing">
    /// <see langword="true"/> when called from <see cref="Dispose()"/>,
    /// <see langword="false"/> when called from the finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        if (disposing)
        {
            _subscriptions.Dispose();

            foreach (var (correlationId, taskCompletionSource) in _taskCompletionSourceMap)
            {
                if (_taskCompletionSourceMap.TryRemove(correlationId, out _))
                    taskCompletionSource.TrySetCanceled();
            }
        }
    }

    /// <summary>
    /// Creates a result representing a successfully completed operation.
    /// </summary>
    protected abstract TServiceResult CreateSuccessResult();

    /// <summary>
    /// Creates a result representing a failed operation.
    /// </summary>
    /// <param name="errorMessage">Human-readable description of the failure.</param>
    /// <param name="errorCode">Optional backend error code.</param>
    protected abstract TServiceResult CreateErrorResult(string errorMessage, int? errorCode = null);

    private TServiceResult CreateErrorResult(ErrorInfo errorInfo)
        => CreateErrorResult(errorInfo.Message ?? CommonPhrases.AnUnknownErrorOccurred, errorInfo.ErrorCode);

    /// <summary>
    /// Sends <paramref name="command"/> and waits for the correlated backend event to arrive
    /// within <see cref="IUiMediator.CommandTimeoutMs"/> milliseconds.
    /// </summary>
    /// <typeparam name="TCommand">A command type that carries a <see cref="Guid"/> correlation ID.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    /// <returns>
    /// <see cref="CreateSuccessResult()"/> on success, or <see cref="CreateErrorResult(string, int?)"/>
    /// on backend error or timeout.
    /// </returns>
    protected async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletion(command, CreateErrorResult, null, cancellationToken);

    /// <summary>
    /// Sends <paramref name="command"/> and waits for the correlated backend event,
    /// using a custom <paramref name="errorOccured"/> factory instead of <see cref="CreateErrorResult(string, int?)"/>.
    /// </summary>
    /// <typeparam name="TCommand">A command type that carries a <see cref="Guid"/> correlation ID.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="errorOccured">Factory invoked with the backend <see cref="ErrorInfo"/> when the operation fails.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    protected async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Func<ErrorInfo, TServiceResult> errorOccured, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletion(command, errorOccured, null, cancellationToken);

    /// <summary>
    /// Sends <paramref name="command"/> and, after dispatching, executes <paramref name="afterSend"/>
    /// before waiting for the correlated backend event.
    /// </summary>
    /// <typeparam name="TCommand">A command type that carries a <see cref="Guid"/> correlation ID.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="afterSend">Optional async continuation to run immediately after the command is sent.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    protected async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Func<CancellationToken, Task>? afterSend = null, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletion(command, CreateErrorResult, afterSend, cancellationToken);

    private async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Func<ErrorInfo, TServiceResult> errorOccured, Func<CancellationToken, Task>? afterSend = null, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletionInternal(command.CorrelationId, errorOccured, async ct =>
        {
            await Mediator.Send(command, ct);

            if (afterSend != null)
                await afterSend(ct);

        }, cancellationToken);

    /// <summary>
    /// Sends an instance-scoped <paramref name="command"/> and waits for the correlated backend event.
    /// </summary>
    /// <typeparam name="TCommand">An instance-dependent command type that carries a <see cref="Guid"/> correlation ID.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="instanceId">The target instance ID forwarded to <see cref="IUiMediator"/>.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    protected async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Guid instanceId, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletion(command, instanceId, CreateErrorResult, null, cancellationToken);

    /// <summary>
    /// Sends an instance-scoped <paramref name="command"/> and waits for the correlated backend event,
    /// using a custom <paramref name="errorOccured"/> factory.
    /// </summary>
    /// <typeparam name="TCommand">An instance-dependent command type that carries a <see cref="Guid"/> correlation ID.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="instanceId">The target instance ID forwarded to <see cref="IUiMediator"/>.</param>
    /// <param name="errorOccured">Factory invoked with the backend <see cref="ErrorInfo"/> when the operation fails.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    protected async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Guid instanceId, Func<ErrorInfo, TServiceResult> errorOccured, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand, CorrelatedBy<Guid>
        => await SendAndWaitForCompletion(command, instanceId, errorOccured, null, cancellationToken);

    private async Task<TServiceResult> SendAndWaitForCompletion<TCommand>(TCommand command, Guid instanceId, Func<ErrorInfo, TServiceResult> errorOccured, Func<CancellationToken, Task>? afterSend = null, CancellationToken cancellationToken = default)
        where TCommand : class, IInstanceDependentCommand, CorrelatedBy<Guid> => await SendAndWaitForCompletionInternal(command.CorrelationId, errorOccured, async ct =>
        {
            await Mediator.Send(command, instanceId, ct);

            if (afterSend != null)
                await afterSend(ct);

        }, cancellationToken);

    private async Task<TServiceResult> SendAndWaitForCompletionInternal(Guid correlationId, Func<ErrorInfo, TServiceResult> errorOccured, Func<CancellationToken, Task> processCall,
        CancellationToken cancellationToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource<ErrorInfo?>();
        _taskCompletionSourceMap[correlationId] = taskCompletionSource;

        // Post-add guard: Dispose may have set _disposed and completed its drain
        // between the pre-check and the map insertion above.
        if (_disposed)
        {
            _taskCompletionSourceMap.TryRemove(correlationId, out _);
            taskCompletionSource.TrySetCanceled(cancellationToken);
        }

        try
        {
            if (!_disposed)
            {
                try
                {
                    await processCall(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Ensure the caller receives a proper ISaveResult on failure
                    return CreateErrorResult(new ErrorInfo(0, ex.Message));
                }
            }

            return await WaitForCommandCompletion(taskCompletionSource, errorOccured, cancellationToken);
        }
        finally
        {
            _taskCompletionSourceMap.TryRemove(correlationId, out _);
        }
    }

    private async Task<TServiceResult> WaitForCommandCompletion(TaskCompletionSource<ErrorInfo?> taskCompletionSource, Func<ErrorInfo, TServiceResult> errorOccured,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var errorInfo = await taskCompletionSource.Task.WaitAsync(TimeSpan.FromMilliseconds(Mediator.CommandTimeoutMs), cancellationToken);
            if (errorInfo is not null)
                return errorOccured.Invoke(errorInfo);

            return CreateSuccessResult();
        }
        catch (OperationCanceledException)
        {
            // WaitAsync throws OperationCanceledException/TaskCanceledException when a task is cancelled
            return CreateSuccessResult();
        }
        catch (TimeoutException)
        {
            return CreateErrorResult(CommonPhrases.TheOperationHasTimedOut);
        }
    }
}
