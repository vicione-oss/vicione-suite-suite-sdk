using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sdk.Modules;

namespace Sdk.Client.Services;

/// <summary>
/// Defines a contract for JavaScript interoperability services.
/// </summary>
public interface IJsInterop
{
    /// <summary>
    /// Checks if a given HTML form is valid.
    /// </summary>
    Task<bool> FormValid(ElementReference form, CancellationToken token = default);

    /// <summary>
    /// Gets the value of a cookie by its name.
    /// </summary>
    Task<string> GetCookie(string name, CancellationToken token = default);

    /// <summary>
    /// Gets the value of an element by its 'name' attribute.
    /// </summary>
    Task<string> GetElementByName(string name, CancellationToken token = default);

    /// <summary>
    /// Gets the list of selected files from a file input element.
    /// </summary>
    Task<string[]> GetFiles(string id, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes a 'link' element in the document's head.
    /// </summary>
    Task IncludeLink(string id, string rel, Uri href, string type, string integrity, string crossorigin, string key, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes multiple 'link' elements in the document's head.
    /// </summary>
    Task IncludeLinks(object[] links, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes a 'meta' tag in the document's head.
    /// </summary>
    Task IncludeMeta(string id, string attribute, string name, string content, string key, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes a 'script' element in the document.
    /// </summary>
    Task IncludeScript(string id, Uri src, string integrity, string crossorigin, string content, string location, string key, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes a JavaScript module script.
    /// </summary>
    Task<IJSObjectReference?> IncludeModuleScript(Uri location, CancellationToken token = default);

    /// <summary>
    /// Dynamically includes a JavaScript module script associated with a specific module.
    /// </summary>
    Task<IJSObjectReference?> IncludeModuleScript<T>(string filename, CancellationToken token = default) where T : IModule;

    /// <summary>
    /// Dynamically includes multiple 'script' elements in the document.
    /// </summary>
    Task IncludeScripts(object[] scripts, CancellationToken token = default);

    /// <summary>
    /// Redirects the browser to a new URL after a specified delay.
    /// </summary>
    Task RedirectBrowser(Uri url, int wait, CancellationToken token = default);

    /// <summary>
    /// Refreshes the browser after a specified delay.
    /// </summary>
    Task RefreshBrowser(bool force, int wait, CancellationToken token = default);

    /// <summary>
    /// Removes a range of elements from the DOM based on their IDs.
    /// </summary>
    Task RemoveElementsById(string prefix, string first, string last, CancellationToken token = default);

    /// <summary>
    /// Removes 'script' elements from the DOM that match a specific source URL.
    /// </summary>
    Task RemoveScriptsBySource(Uri source, CancellationToken token = default);

    /// <summary>
    /// Sets a browser cookie.
    /// </summary>
    Task SetCookie(string name, string value, int days, CancellationToken token = default);

    /// <summary>
    /// Sets an attribute on a DOM element.
    /// </summary>
    Task SetElementAttribute(string id, string attribute, string value, CancellationToken token = default);

    /// <summary>
    /// Submits a form programmatically via a POST request.
    /// </summary>
    Task SubmitForm(string path, object fields, CancellationToken token = default);

    /// <summary>
    /// Updates the title of the HTML document.
    /// </summary>
    Task UpdateTitle(string title, CancellationToken token = default);

    /// <summary>
    /// Uploads files from a file input element to a specified URL.
    /// </summary>
    Task UploadFiles(string posturl, string folder, string id, CancellationToken token = default);

    /// <summary>
    /// Triggers a file download in the browser with the given string content.
    /// </summary>
    Task DownloadAs(string content, string name, CancellationToken token = default);

    /// <summary>
    /// Triggers a file download in the browser with the given stream content.
    /// </summary>
    Task DownloadAs(Stream content, string name, CancellationToken token = default);
}
