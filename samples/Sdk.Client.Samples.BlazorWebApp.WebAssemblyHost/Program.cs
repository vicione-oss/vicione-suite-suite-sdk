using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sdk.Client.Samples.Shared.Extensions;
using ViciOne.Ui.MonochromeIcons.Assets.Extensions;

// This is the browser side of the Blazor Web App in Interactive WebAssembly mode: the server serves the page, then this
// program starts in the browser and runs the sample components. Its services are separate from the server's, so every
// service a component injects must be registered here too.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddShared();

// In the browser there are no icon files on disk, so icons are fetched by URL. The URL-based provider resolves
// _content/... relative to this HttpClient's base address and yields no icon without one.
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddUrlBasedMonochromeIconSvgMarkupProvider();

await builder.Build().RunAsync();
