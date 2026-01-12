using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides safe extension methods for <see cref="IJSObjectReference"/>.
/// </summary>
public static partial class IJSObjectReferenceExtensions
{
    /// <summary>
    /// Safely invokes a JavaScript function that does not return a value, swallowing any <see cref="JSDisconnectedException"/>.
    /// </summary>
    public static async Task TryInvokeVoidAsync(this IJSObjectReference? objectReference, string jsMethodName, ILogger logger)
    {
        if (objectReference == null)
            return;

        try
        {
            // Microsoft.JSInterop.JSDisconnectedException: JavaScript interop calls cannot be issued at this time.
            // This is because the circuit has disconnected and is being disposed.
            // This was "solved" by https://github.com/danroth27/BestForYouRecipes/commit/8a54651e8d57337ed8e76576dd18a3efd962e215

            // TODO: seems to be fixed but we need to keep an eye on it
            await objectReference.InvokeVoidAsync(jsMethodName).ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            // swallow it https://github.com/dotnet/aspnetcore/issues/49376
        }
        catch (Exception ex)
        {
            LogInvokingAsyncJsMethodFailed(logger, ex, jsMethodName);
        }
    }

    /// <summary>
    /// Safely invokes a JavaScript function that returns a value, swallowing any <see cref="JSDisconnectedException"/>.
    /// </summary>
    public static async ValueTask<TResult?> TryInvokeAsync<TResult>(this IJSObjectReference? objectReference, string jsMethodName, ILogger logger)
    {
        if (objectReference == null)
            return default;

        try
        {
            // Microsoft.JSInterop.JSDisconnectedException: JavaScript interop calls cannot be issued at this time.
            // This is because the circuit has disconnected and is being disposed.
            // This was "solved" by https://github.com/danroth27/BestForYouRecipes/commit/8a54651e8d57337ed8e76576dd18a3efd962e215

            // TODO: seems to be fixed but we need to keep an eye on it
            return await objectReference.InvokeAsync<TResult>(jsMethodName).ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            // swallow it https://github.com/dotnet/aspnetcore/issues/49376
        }
        catch (Exception ex)
        {
            LogInvokingAsyncJsMethodFailed(logger, ex, jsMethodName);
        }

        return default;
    }

    /// <summary>
    /// Safely disposes of the JavaScript object reference, swallowing any <see cref="JSDisconnectedException"/>.
    /// </summary>
    public static async Task TryDisposeAsync(this IJSObjectReference? objectReference, ILogger logger)
    {
        if (objectReference == null)
            return;

        try
        {
            await objectReference.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // swallow it https://github.com/dotnet/aspnetcore/issues/49376
        }
        catch (Exception ex)
        {
            LogDisposingJsObjectReferenceFailed(logger, ex);
        }
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Invoking js.{Call}() failed")]
    internal static partial void LogInvokingAsyncJsMethodFailed(ILogger logger, Exception exception, string call);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = $"Disposing {nameof(IJSObjectReference)} failed")]
    internal static partial void LogDisposingJsObjectReferenceFailed(ILogger logger, Exception exception);
}
