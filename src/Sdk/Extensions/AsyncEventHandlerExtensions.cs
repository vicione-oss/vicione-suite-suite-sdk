namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods to safely invoke asynchronous event handlers (<see cref="Func{TResult}"/> delegates),
/// ensuring that all subscribers are awaited and any exceptions are aggregated.
/// </summary>
public static class AsyncEventHandlerExtensions
{
    /// <summary>
    /// Invokes every subscriber of <paramref name="event"/> in subscription order, awaiting each; <see langword="null"/> does nothing.
    /// </summary>
    /// <exception cref="AggregateException">Thrown after all handlers ran if any of them threw; it holds every exception.</exception>
    public static async Task Invoke<TEventArgs1, TEventArgs2>(
        this Func<TEventArgs1, TEventArgs2, Task>? @event,
        TEventArgs1 args1,
        TEventArgs2 args2)
    {
        if (@event is null)
            return;

        var handlers = @event.GetInvocationList().OfType<Func<TEventArgs1, TEventArgs2, Task>>();
        List<Exception>? exceptions = null;

        foreach (var handler in handlers)
        {
            try
            {
                await handler(args1, args2).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }

    /// <inheritdoc cref="Invoke{TEventArgs1, TEventArgs2}(Func{TEventArgs1, TEventArgs2, Task}?, TEventArgs1, TEventArgs2)"/>
    public static async Task Invoke<TEventArgs>(this Func<TEventArgs, Task>? @event, TEventArgs args)
    {
        if (@event is null)
            return;

        var handlers = @event.GetInvocationList().OfType<Func<TEventArgs, Task>>();
        List<Exception>? exceptions = null;

        foreach (var handler in handlers)
        {
            try
            {
                await handler(args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }

    /// <inheritdoc cref="Invoke{TEventArgs1, TEventArgs2}(Func{TEventArgs1, TEventArgs2, Task}?, TEventArgs1, TEventArgs2)"/>
    public static async Task Invoke(this Func<Task>? @event)
    {
        if (@event is null)
            return;

        var handlers = @event.GetInvocationList().OfType<Func<Task>>();
        List<Exception>? exceptions = null;

        foreach (var handler in handlers)
        {
            try
            {
                await handler().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }
}

