using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sdk.Client.Samples.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddShared();

var host = builder.Build();

await host.RunAsync();
