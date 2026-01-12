using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sdk.Backend.Modules;

namespace Sdk.Testing.Backend;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkspaceService<TModule>(this IServiceCollection services, Action<IWorkspaceProvider<TModule>>? setup = null)
        where TModule : BackendModule
    {
        var workspace = Substitute.For<IWorkspaceProvider<TModule>>();
        services.AddSingleton(workspace);

        if (setup is null)
        {
            var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            if (string.IsNullOrEmpty(location))
                return services;

            workspace.Home.Returns(Path.Combine(location, "home"));
            workspace.Cache.Returns(Path.Combine(location, "cache"));
        }
        else
            setup.Invoke(workspace);

        return services;
    }

    public static IServiceCollection AddMvcBuilder(this IServiceCollection services, Action<IMvcBuilder>? setup = null)
    {
        var mvcBuilder = Substitute.For<IMvcBuilder>();
        mvcBuilder.PartManager.Returns(new Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager());
        services.AddSingleton(mvcBuilder);

        setup?.Invoke(mvcBuilder);

        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, Dictionary<string, string?>? customSettings = null)
    {
        services.AddSingleton(new TestConfig().AddCustomSettings(customSettings).BuildConfiguration());

        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, TestConfig config)
    {
        services.AddSingleton(config.BuildConfiguration());

        return services;
    }

    public static IServiceCollection AddEndpointRouteBuilder(this IServiceCollection services, Action<IEndpointRouteBuilder>? setup = null)
    {
        var endpointBuilder = Substitute.For<IEndpointRouteBuilder>();
        services.AddSingleton(endpointBuilder);

        setup?.Invoke(endpointBuilder);

        return services;
    }

    public static IServiceCollection AddMassTransitConfigurators(this IServiceCollection services,
        Action<IBusRegistrationConfigurator>? setup = null)
    {
        var busRegistration = Substitute.For<IBusRegistrationConfigurator>();
        var sagaRegistration = Substitute.For<ISagaRegistrationConfigurator>();

        services.AddSingleton(busRegistration);
        services.AddSingleton(sagaRegistration);

        setup?.Invoke(busRegistration);

        return services;
    }

    public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration config)
    {
        var configDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfiguration));
        if (configDescriptor is not null)
            services.Remove(configDescriptor);

        services.AddSingleton(config);

        return services;
    }
}
