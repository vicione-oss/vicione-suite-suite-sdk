namespace Sdk.Extensions;

/// <summary>
/// Provides extension methods to safely invoke asynchronous event handlers (<see cref="Func{TResult}"/> delegates),
/// ensuring that all subscribers are awaited and any exceptions are aggregated.
/// </summary>
public static class AsyncEventHandlerExtensions
{
    /// <summary>
    /// Invokes an asynchronous event with two arguments, awaiting all subscribers.
    /// </summary>
    /// <param name="event">
    /// The event delegate to invoke. May be <see langword="null"/>, in which case nothing is invoked.
    /// </param>
    /// <param name="args1">The first argument to pass to each event handler.</param>
    /// <param name="args2">The second argument to pass to each event handler.</param>
    /// <returns>
    /// A task representing the asynchronous invocation of all handlers.
    /// </returns>
    /// <remarks>
    /// All handlers are invoked sequentially in the order they were subscribed.
    ///
    /// <para>
    /// If one or more handlers throw exceptions, all exceptions are collected and thrown together
    /// as an <see cref="AggregateException"/> after all handlers have been invoked.
    /// </para>
    /// </remarks>
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

    /// <summary>
    /// Invokes an asynchronous event with one argument, awaiting all subscribers.
    /// </summary>
    /// <param name="event">
    /// The event delegate to invoke. May be <see langword="null"/>, in which case nothing is invoked.
    /// </param>
    /// <param name="args">The argument to pass to each event handler.</param>
    /// <returns>
    /// A task representing the asynchronous invocation of all handlers.
    /// </returns>
    /// <remarks>
    /// All handlers are invoked sequentially in the order they were subscribed.
    ///
    /// <para>
    /// If one or more handlers throw exceptions, all exceptions are collected and thrown together
    /// as an <see cref="AggregateException"/> after all handlers have been invoked.
    /// </para>
    /// </remarks>
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

    /// <summary>
    /// Invokes an asynchronous event with no arguments, awaiting all subscribers.
    /// </summary>
    /// <param name="event">
    /// The event delegate to invoke. May be <see langword="null"/>, in which case nothing is invoked.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous invocation of all handlers.
    /// </returns>
    /// <remarks>
    /// All handlers are invoked sequentially in the order they were subscribed.
    ///
    /// <para>
    /// If one or more handlers throw exceptions, all exceptions are collected and thrown together
    /// as an <see cref="AggregateException"/> after all handlers have been invoked.
    /// </para>
    /// </remarks>
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

