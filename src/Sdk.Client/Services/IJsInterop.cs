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
    Task<bool> FormValid(ElementReference form);

    /// <summary>
    /// Gets the value of a cookie by its name.
    /// </summary>
    Task<string> GetCookie(string name);

    /// <summary>
    /// Gets the value of an element by its 'name' attribute.
    /// </summary>
    Task<string> GetElementByName(string name);

    /// <summary>
    /// Gets the list of selected files from a file input element.
    /// </summary>
    Task<string[]> GetFiles(string id);

    /// <summary>
    /// Dynamically includes a 'link' element in the document's head.
    /// </summary>
    Task IncludeLink(string id, string rel, string href, string type, string integrity, string crossorigin, string key);

    /// <summary>
    /// Dynamically includes multiple 'link' elements in the document's head.
    /// </summary>
    Task IncludeLinks(object[] links);

    /// <summary>
    /// Dynamically includes a 'meta' tag in the document's head.
    /// </summary>
    Task IncludeMeta(string id, string attribute, string name, string content, string key);

    /// <summary>
    /// Dynamically includes a 'script' element in the document.
    /// </summary>
    Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key);

    /// <summary>
    /// Dynamically includes a JavaScript module script.
    /// </summary>
    Task<IJSObjectReference?> IncludeModuleScript(string location);

    /// <summary>
    /// Dynamically includes a JavaScript module script associated with a specific module.
    /// </summary>
    Task<IJSObjectReference?> IncludeModuleScript<T>(string filename) where T : IModule;

    /// <summary>
    /// Dynamically includes multiple 'script' elements in the document.
    /// </summary>
    Task IncludeScripts(object[] scripts);

    /// <summary>
    /// Redirects the browser to a new URL after a specified delay.
    /// </summary>
    Task RedirectBrowser(Uri url, int wait);

    /// <summary>
    /// Refreshes the browser after a specified delay.
    /// </summary>
    Task RefreshBrowser(bool force, int wait);

    /// <summary>
    /// Removes a range of elements from the DOM based on their IDs.
    /// </summary>
    Task RemoveElementsById(string prefix, string first, string last);

    /// <summary>
    /// Removes 'script' elements from the DOM that match a specific source URL.
    /// </summary>
    Task RemoveScriptsBySource(string source);

    /// <summary>
    /// Sets a browser cookie.
    /// </summary>
    Task SetCookie(string name, string value, int days);

    /// <summary>
    /// Sets an attribute on a DOM element.
    /// </summary>
    Task SetElementAttribute(string id, string attribute, string value);

    /// <summary>
    /// Submits a form programmatically via a POST request.
    /// </summary>
    Task SubmitForm(string path, object fields);

    /// <summary>
    /// Updates the title of the HTML document.
    /// </summary>
    Task UpdateTitle(string title);

    /// <summary>
    /// Uploads files from a file input element to a specified URL.
    /// </summary>
    Task UploadFiles(string posturl, string folder, string id);

    /// <summary>
    /// Triggers a file download in the browser with the given string content.
    /// </summary>
    Task DownloadAs(string content, string name);

    /// <summary>
    /// Triggers a file download in the browser with the given stream content.
    /// </summary>
    Task DownloadAs(Stream content, string name);
}
