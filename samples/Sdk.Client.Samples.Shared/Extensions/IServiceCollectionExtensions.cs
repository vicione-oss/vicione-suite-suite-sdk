using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.Samples.Shared.Services;
using Sdk.Client.Services;

namespace Sdk.Client.Samples.Shared.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddScoped<IJsInterop, JsInterop>();

        return services;
    }
}
