using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sdk.Client.Samples.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddShared();

await builder.Build().RunAsync();
