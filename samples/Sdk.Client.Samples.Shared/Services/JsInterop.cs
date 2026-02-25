using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;
using Sdk.Client.Services;
using Sdk.Modules;

namespace Sdk.Client.Samples.Shared.Services;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class JsInterop(IJSRuntime jsRuntime) : IJsInterop
{
    public Task DownloadAs(string content, string name, CancellationToken token = default) => throw new NotImplementedException();
    public Task DownloadAs(Stream content, string name, CancellationToken token = default) => throw new NotImplementedException();
    public Task<bool> FormValid(ElementReference form, CancellationToken token = default) => throw new NotImplementedException();
    public Task<string> GetCookie(string name, CancellationToken token = default) => throw new NotImplementedException();
    public Task<string> GetElementByName(string name, CancellationToken token = default) => throw new NotImplementedException();
    public Task<string[]> GetFiles(string id, CancellationToken token = default) => throw new NotImplementedException();
    public Task IncludeLink(string id, string rel, Uri href, string type, string integrity, string crossorigin, string key, CancellationToken token = default) => throw new NotImplementedException();
    public Task IncludeLinks(object[] links, CancellationToken token = default) => throw new NotImplementedException();
    public Task IncludeMeta(string id, string attribute, string name, string content, string key, CancellationToken token = default) => throw new NotImplementedException();
    public Task IncludeScript(string id, Uri src, string integrity, string crossorigin, string content, string location, string key, CancellationToken token = default) => throw new NotImplementedException();
    public async Task<IJSObjectReference?> IncludeModuleScript(Uri location, CancellationToken token = default) => await jsRuntime.ImportScript(location);
    public Task<IJSObjectReference?> IncludeModuleScript<T>(string filename, CancellationToken token = default) where T : IModule => throw new NotImplementedException();
    public Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key, CancellationToken token = default) => throw new NotImplementedException();
    public Task IncludeScripts(object[] scripts, CancellationToken token = default) => throw new NotImplementedException();
    public Task RedirectBrowser(Uri url, int wait, CancellationToken token = default) => throw new NotImplementedException();
    public Task RefreshBrowser(bool force, int wait, CancellationToken token = default) => throw new NotImplementedException();
    public Task RemoveElementsById(string prefix, string first, string last, CancellationToken token = default) => throw new NotImplementedException();
    public Task RemoveScriptsBySource(Uri source, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetCookie(string name, string value, int days, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetElementAttribute(string id, string attribute, string value, CancellationToken token = default) => throw new NotImplementedException();
    public Task SubmitForm(string path, object fields, CancellationToken token = default) => throw new NotImplementedException();
    public Task UpdateTitle(string title, CancellationToken token = default) => throw new NotImplementedException();
    public Task UploadFiles(string posturl, string folder, string id, CancellationToken token = default) => throw new NotImplementedException();
}
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore IDE0079 // Remove unnecessary suppression
