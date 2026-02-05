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

    extension(IJSRuntime jsRuntime)
    {
        /// <summary>
        /// Dynamically loads one or more CSS files into the document's head by using a helper JavaScript module.
        /// </summary>
        public async Task<IJSObjectReference> IncludeCssPaths(IEnumerable<string> cssFilePaths)
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
        public async Task<IJSObjectReference> ImportScript(Uri jsUrl)
            => await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsUrl.AbsolutePath).AsTask();

        /// <summary>
        /// Imports a JavaScript module from the application's global 'wwwroot/js' folder.
        /// </summary>
        public async Task<IJSObjectReference> ImportGlobalScript(string jsFilename)
        {
            var jsPath = ModuleAssetHelper.GetGlobalJsPath(jsFilename);

            return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
        }

        /// <summary>
        /// Imports a JavaScript module from a specific module's 'wwwroot/js' folder.
        /// </summary>
        public async Task<IJSObjectReference> ImportModuleScript<T>(string jsFilename)
            where T : IModule
        {
            var jsPath = ModuleAssetHelper.GetModuleJsPath<T>(jsFilename);

            return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
        }
    }
}
