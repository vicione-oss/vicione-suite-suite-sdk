using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Modules;
using Sdk.Instance;
using TestModule.Backend.Services;

namespace TestModule.Backend;

public class TestBackendModule(IModuleInitializer? initializer) : BackendModule
{
    public override IModuleInitializer? ModuleInitializer { get; } = initializer;

    public event EventHandler<string>? CallReceived;

    public TestBackendModule() : this(null)
    {
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder)
    {
        services.AddSingleton<IInstanceInformationProvider, TestInstanceInformationProvider>();
        CallReceived?.Invoke(this, nameof(ConfigureServices));
    }

    public override void UseServices(IApplicationBuilder app)
        => CallReceived?.Invoke(this, nameof(UseServices));

    public override void MapEndpoints(IEndpointRouteBuilder endpoints)
        => CallReceived?.Invoke(this, nameof(MapEndpoints));

    public override void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType)
        => CallReceived?.Invoke(this, nameof(ConfigureMessageBus));
}
