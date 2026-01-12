using Microsoft.JSInterop;
using Sdk.Client.Modules;
using Sdk.Modules;

namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IJSRuntime"/> to simplify JavaScript interop calls.
/// </summary>
public static class JSRuntimeExtensions
{
    private const string JsImportCommand = "import";
    private const string CssLoaderScript = "css-loader.js";
    private const string CssLoaderCall = "includeCss";

    /// <summary>
    /// Dynamically loads one or more CSS files into the document's head by using a helper JavaScript module.
    /// </summary>
    public static async Task<IJSObjectReference> IncludeCssPaths(this IJSRuntime jsRuntime, IEnumerable<string> cssFilePaths)
    {
        var module = await jsRuntime.ImportGlobalScript(CssLoaderScript);

        foreach (var filename in cssFilePaths)
        {
            await module.InvokeAsync<object>(CssLoaderCall, filename);
        }

        return module;
    }

    /// <summary>
    /// Imports a JavaScript module from the specified <see cref="Uri"/>.
    /// </summary>
    public static async Task<IJSObjectReference> ImportScript(this IJSRuntime jsRuntime, Uri jsUrl)
        => await jsRuntime.ImportScript(jsUrl.AbsolutePath);

    /// <summary>
    /// Imports a JavaScript module from the specified URL path.
    /// </summary>
    public static async Task<IJSObjectReference> ImportScript(this IJSRuntime jsRuntime, string jsUrl)
        => await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsUrl).AsTask();

    /// <summary>
    /// Imports a JavaScript module from the application's global 'wwwroot/js' folder.
    /// </summary>
    public static async Task<IJSObjectReference> ImportGlobalScript(this IJSRuntime jsRuntime, string jsFilename)
    {
        var jsPath = ModuleAssetHelper.GetGlobalJsPath(jsFilename);

        return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
    }

    /// <summary>
    /// Imports a JavaScript module from a specific module's 'wwwroot/js' folder.
    /// </summary>
    public static async Task<IJSObjectReference> ImportModuleScript<T>(this IJSRuntime jsRuntime, string jsFilename)
        where T : IModule
    {
        var jsPath = ModuleAssetHelper.GetModuleJsPath<T>(jsFilename);

        return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
    }
}
