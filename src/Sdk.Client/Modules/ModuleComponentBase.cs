using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Contracts;
using Sdk.Client.Services;

namespace Sdk.Client.Modules;

public abstract partial class ModuleComponentBase<TComponent> : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJsInterop JsInterop { get; set; } = default!;

    [Inject] protected ILogger<TComponent> Logger { get; set; } = default!;

    /// <summary>
    /// Resources will be loaded via JSInterop on OnAfterRenderAsync
    /// if firstRender=true
    /// </summary>
    protected virtual List<Resource>? Resources => null;

    // base lifecycle method for handling JSInterop script registration
    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            LogOnAfterRenderAsync(Logger, GetType().Name);

            await LoadComponentResources(Resources);

            await OnAfterResourcesLoadedAsync();
        }

        await OnAfterRenderedAsync(firstRender);
    }

    /// <summary>
    /// Method invoked after the resources were loaded on first render
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnAfterResourcesLoadedAsync() => Task.CompletedTask;

    /// <summary>
    /// Method invoked after sealed OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected virtual Task OnAfterRenderedAsync(bool firstRender) => Task.CompletedTask;


    private async Task LoadComponentResources(List<Resource>? resources)
    {
        if (resources is null)
            return;

        foreach (var resource in resources.Where(item => item.ResourceType == ResourceType.Stylesheet))
        {
            if (string.IsNullOrEmpty(resource.Id))
                continue;

            await JsInterop.IncludeLink(resource.Id, "stylesheet", resource.Url ?? "", "text/css", "", "", "");

            LogIncludeLink(Logger, GetType().Name, resource);
        }

        // global scripts won't be removed and loading it into one operation was necessary to find the bundle issue
        await IncludeScripts(resources, ResourceDeclaration.Global);

        await IncludeScripts(resources, ResourceDeclaration.Local);
    }

    private async Task IncludeScripts(List<Resource> resources, ResourceDeclaration declaration)
    {
        var scripts = resources
            .Where(item => item.ResourceType == ResourceType.Script && item.Declaration == declaration)
            .Select(r => new
            {
                id = r.Id,
                href = r.Url,
                bundle = r.Bundle ?? "",
                integrity = r.Integrity ?? "",
                crossorigin = r.CrossOrigin ?? ""
            })
            .ToArray<object>();

        if (scripts.Length != 0)
        {
            await JsInterop.IncludeScripts(scripts);

            LogIncludeScripts(Logger, GetType().Name, declaration, string.Join(", ", scripts));
        }
    }

    private async Task UnloadComponentResources(List<Resource>? resources)
    {
        if (resources is null)
            return;

        foreach (var resource in resources.Where(item => item.Declaration != ResourceDeclaration.Global))
        {
            if (resource.ResourceType == ResourceType.Stylesheet && !string.IsNullOrEmpty(resource.Id))
            {
                await JsInterop.RemoveElementsById(resource.Id, string.Empty, string.Empty);

                LogUnloadComponentResourceRemoveElementsById(Logger, GetType().Name, resource);

                continue;
            }

            if (resource.ResourceType == ResourceType.Script && !string.IsNullOrEmpty(resource.Url))
            {
                await JsInterop.RemoveScriptsBySource(resource.Url);

                LogUnloadComponentResourceRemoveScriptsBySource(Logger, GetType().Name, resource);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await UnloadComponentResources(Resources);
        }
        catch (JSDisconnectedException)
        {
            // swallow it https://github.com/dotnet/aspnetcore/issues/49376
        }
        catch (Exception ex)
        {
            LogUnloadComponentResourcesFailed(Logger, ex, GetType().Name);
        }

        try
        {
            await DisposeInternal();
        }
        catch (JSDisconnectedException)
        {
            // swallow it https://github.com/dotnet/aspnetcore/issues/49376
        }
        catch (Exception ex)
        {
            LogDisposeInternalFailed(Logger, ex, GetType().Name);
        }

        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeInternal() => ValueTask.CompletedTask;

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "{className} - OnAfterRenderAsync loading resources on first render")]
    private static partial void LogOnAfterRenderAsync(ILogger<TComponent> logger, string className);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "{className} - included link {resource}")]
    private static partial void LogIncludeLink(ILogger<TComponent> logger, string className, Resource resource);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "{className} - included scripts [{declaration}]: {scripts}")]
    private static partial void LogIncludeScripts(ILogger<TComponent> logger, string className, ResourceDeclaration declaration, string scripts);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "{className} - RemoveElementsById {resource}")]
    private static partial void LogUnloadComponentResourceRemoveElementsById(ILogger<TComponent> logger, string className, Resource resource);

    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "{className} - Removed {resource}")]
    private static partial void LogUnloadComponentResourceRemoveScriptsBySource(ILogger<TComponent> logger, string className, Resource resource);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "{className} - Unloading component resources failed")]
    private static partial void LogUnloadComponentResourcesFailed(ILogger<TComponent> logger, Exception exception, string className);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "{className} - Disposing internal resources failed")]
    private static partial void LogDisposeInternalFailed(ILogger<TComponent> logger, Exception exception, string className);
}
