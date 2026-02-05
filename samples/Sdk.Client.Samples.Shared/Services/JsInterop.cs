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
    public Task DownloadAs(string content, string name) => throw new NotImplementedException();
    public Task DownloadAs(Stream content, string name) => throw new NotImplementedException();
    public Task<bool> FormValid(ElementReference form) => throw new NotImplementedException();
    public Task<string> GetCookie(string name) => throw new NotImplementedException();
    public Task<string> GetElementByName(string name) => throw new NotImplementedException();
    public Task<string[]> GetFiles(string id) => throw new NotImplementedException();
    public Task IncludeLink(string id, string rel, string href, string type, string integrity, string crossorigin, string key) => throw new NotImplementedException();
    public Task IncludeLinks(object[] links) => throw new NotImplementedException();
    public Task IncludeMeta(string id, string attribute, string name, string content, string key) => throw new NotImplementedException();

    public async Task<IJSObjectReference?> IncludeModuleScript(Uri location)
        => await jsRuntime.ImportScript(location);

    public Task<IJSObjectReference?> IncludeModuleScript<T>(string filename) where T : IModule => throw new NotImplementedException();
    public Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key) => throw new NotImplementedException();
    public Task IncludeScripts(object[] scripts) => throw new NotImplementedException();
    public Task RedirectBrowser(Uri url, int wait) => throw new NotImplementedException();
    public Task RefreshBrowser(bool force, int wait) => throw new NotImplementedException();
    public Task RemoveElementsById(string prefix, string first, string last) => throw new NotImplementedException();
    public Task RemoveScriptsBySource(string source) => throw new NotImplementedException();
    public Task SetCookie(string name, string value, int days) => throw new NotImplementedException();
    public Task SetElementAttribute(string id, string attribute, string value) => throw new NotImplementedException();
    public Task SubmitForm(string path, object fields) => throw new NotImplementedException();
    public Task UpdateTitle(string title) => throw new NotImplementedException();
    public Task UploadFiles(string posturl, string folder, string id) => throw new NotImplementedException();
}
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore IDE0079 // Remove unnecessary suppression
