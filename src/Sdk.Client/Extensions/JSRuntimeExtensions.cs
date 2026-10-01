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
        /// Imports the JavaScript module at <paramref name="jsUrl"/>; an absolute URI is reduced to its path,
        /// a relative one is used as written.
        /// </summary>
        /// <remarks>
        /// A relative URI must start with <c>./</c> or <c>/</c>, as <see cref="ModuleAssetHelper.GetModuleJsUrl{T}"/> does;
        /// the browser rejects <c>_content/…</c> as a bare module specifier. <c>./</c> resolves against the document base.
        /// </remarks>
        /// <exception cref="JSException">The browser cannot load or evaluate the module.</exception>
        public async Task<IJSObjectReference> ImportScript(Uri jsUrl)
            => await jsRuntime.InvokeAsync<IJSObjectReference>(JsImportCommand, jsUrl.RequestPath).AsTask();

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
