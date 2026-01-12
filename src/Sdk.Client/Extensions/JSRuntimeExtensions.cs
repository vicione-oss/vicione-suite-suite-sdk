using Microsoft.JSInterop;
using Sdk.Client.Modules;
using Sdk.Modules;

namespace Sdk.Client.Extensions;

public static class JSRuntimeExtensions
{
    private const string JsImportCommand = "import";
    private const string CssLoaderScript = "css-loader.js";
    private const string CssLoaderCall = "includeCss";

    /// <summary>
    /// call js function to add css links references to cssFilename to page head
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

    public static async Task<IJSObjectReference> ImportScript(this IJSRuntime jsRuntime, Uri jsUrl)
        => await jsRuntime.ImportScript(jsUrl.AbsolutePath);

    /// <summary>
    /// import files from jsUrl directly
    /// </summary>
    public static async Task<IJSObjectReference> ImportScript(this IJSRuntime jsRuntime, string jsUrl)
    {
        return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsUrl).AsTask();
    }

    /// <summary>
    /// import files from client/wwwroot/js
    /// </summary>
    public static async Task<IJSObjectReference> ImportGlobalScript(this IJSRuntime jsRuntime, string jsFilename)
    {
        var jsPath = ModuleAssetHelper.GetGlobalJsPath(jsFilename);

        return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
    }

    /// <summary>
    /// import files from module/wwwroot/js
    /// </summary>
    public static async Task<IJSObjectReference> ImportModuleScript<T>(this IJSRuntime jsRuntime, string jsFilename)
        where T : IModule
    {
        var jsPath = ModuleAssetHelper.GetModuleJsPath<T>(jsFilename);

        return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
    }
}
