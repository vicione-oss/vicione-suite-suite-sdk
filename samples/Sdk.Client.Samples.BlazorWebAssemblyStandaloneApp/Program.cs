using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sdk.Client.Samples.BlazorWebAssemblyStandaloneApp;
using Sdk.Client.Samples.Shared.Extensions;
using ViciOne.Ui.MonochromeIcons.Assets.Extensions;

// A standalone WebAssembly app has no server of its own: wwwroot/index.html loads it, and it runs entirely in the browser.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// App renders into the <div id="app"> of index.html; HeadOutlet lets pages set the <title> and other head content.
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Relative URLs resolve against the address the app was served from.
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddShared();

// In the browser there are no icon files on disk, so icons are fetched by URL, relative to the HttpClient above.
builder.Services.AddUrlBasedMonochromeIconSvgMarkupProvider();

await builder.Build().RunAsync();
