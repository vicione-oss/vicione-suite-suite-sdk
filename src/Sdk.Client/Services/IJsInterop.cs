using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sdk.Modules;

namespace Sdk.Client.Services;

public interface IJsInterop
{
    Task<bool> FormValid(ElementReference form);
    Task<string> GetCookie(string name);
    Task<string> GetElementByName(string name);
    Task<string[]> GetFiles(string id);
    Task IncludeLink(string id, string rel, string href, string type, string integrity, string crossorigin, string key);
    Task IncludeLinks(object[] links);
    Task IncludeMeta(string id, string attribute, string name, string content, string key);
    Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key);
    Task<IJSObjectReference?> IncludeModuleScript(string location);
    Task<IJSObjectReference?> IncludeModuleScript<T>(string filename) where T : IModule;
    Task IncludeScripts(object[] scripts);
    Task RedirectBrowser(Uri url, int wait);
    Task RefreshBrowser(bool force, int wait);
    Task RemoveElementsById(string prefix, string first, string last);
    Task RemoveScriptsBySource(string source);
    Task SetCookie(string name, string value, int days);
    Task SetElementAttribute(string id, string attribute, string value);
    Task SubmitForm(string path, object fields);
    Task UpdateTitle(string title);
    Task UploadFiles(string posturl, string folder, string id);
    Task DownloadAs(string content, string name);
    Task DownloadAs(Stream content, string name);
}
