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
        /// Adds the stylesheets to the document head and returns the loader module that did it.
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
        /// Imports the JavaScript module at the path of <paramref name="jsUrl"/>, which must be absolute.
        /// </summary>
        public async Task<IJSObjectReference> ImportScript(Uri jsUrl)
            => await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsUrl.AbsolutePath).AsTask();

        /// <summary>
        /// Imports a JavaScript module from the application's global <c>js</c> folder.
        /// </summary>
        public async Task<IJSObjectReference> ImportGlobalScript(string jsFilename)
        {
            var jsPath = ModuleAssetHelper.GetGlobalJsUrl(jsFilename);

            return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
        }

        /// <summary>
        /// Imports a JavaScript module from the <c>js</c> folder of module <typeparamref name="T"/>.
        /// </summary>
        public async Task<IJSObjectReference> ImportModuleScript<T>(string jsFilename)
            where T : IModule
        {
            var jsPath = ModuleAssetHelper.GetModuleJsUrl<T>(jsFilename);

            return await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsPath).AsTask();
        }
    }
}
