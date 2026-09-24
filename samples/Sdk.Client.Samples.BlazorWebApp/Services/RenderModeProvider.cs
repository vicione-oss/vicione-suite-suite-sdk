using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Sdk.Client.Samples.BlazorWebApp.Services;

// Holds the render mode App.razor applies to the page head and body. Components choose where they run through their
// render mode: InteractiveServerRenderMode keeps them on the server and updates the browser over a SignalR circuit;
// InteractiveWebAssemblyRenderMode downloads them and runs them in the browser.
internal sealed class RenderModeProvider(bool useWasm = false)
{
    public IComponentRenderMode ContentRenderMode { get; } = useWasm ? new InteractiveWebAssemblyRenderMode(prerender: false) : new InteractiveServerRenderMode();
    public IComponentRenderMode HeaderRenderMode { get; } = useWasm ? new InteractiveWebAssemblyRenderMode(prerender: false) : new InteractiveServerRenderMode();

    public bool UseWebassembly { get; } = useWasm;
}
