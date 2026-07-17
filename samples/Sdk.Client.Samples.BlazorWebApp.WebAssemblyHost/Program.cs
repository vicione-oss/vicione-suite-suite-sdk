using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sdk.Client.Samples.Shared.Extensions;
using ViciOne.Ui.MonochromeIcons.Assets.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddShared();

builder.Services.AddFileSystemBasedMonochromeIconSvgMarkupProvider<Program>();

await builder.Build().RunAsync();
