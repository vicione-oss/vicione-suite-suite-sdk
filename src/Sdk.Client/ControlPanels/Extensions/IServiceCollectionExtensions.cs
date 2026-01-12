using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Authorization.Extensions;
using Sdk.Client.ControlPanels.Builders;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register services related to control panels.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary services for a control panel and returns a builder for further configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>An <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}"/> for advanced configuration of the control panel.</returns>
    public static IControlPanelBuilder<TClientModule, TControlPanel, TState> AddControlPanel<TClientModule, TControlPanel, TState>(
        this IServiceCollection services)
            where TClientModule : class, IClientModule
            where TControlPanel : ControlPanelBase<TState>
            where TState : class, IControlPanelState
    {
        services.AddControlPanelCore<TClientModule>();

        var builder = new ControlPanelBuilder<TClientModule, TControlPanel, TState>(services);

        return builder;
    }

    /// <summary>
    /// Adds the core services required for control panel functionality, such as the registries.
    /// </summary>
    /// <remarks>
    /// This method ensures that <see cref="IControlPanelRegistry{TClientModule}"/> and <see cref="IControlPanelPageRegistry"/>
    /// are registered in the DI container.
    /// </remarks>
    public static IServiceCollection AddControlPanelCore<TClientModule>(this IServiceCollection services)
            where TClientModule : class, IClientModule
    {
        services.AddControlPanelRegistry<TClientModule>();

        return services;
    }

    internal static IServiceCollection AddControlPanelDescriptor(this IServiceCollection services,
        ControlPanelInfo controlPanelInfo)
    {
        var descriptorInterfaceType = typeof(IControlPanelDescriptor<>).MakeGenericType(controlPanelInfo.ComponentType);

        if (descriptorInterfaceType.IsAssignableFrom(controlPanelInfo.DescriptorType))
            services.AddKeyedScoped(descriptorInterfaceType, controlPanelInfo.KeyedServiceKey, controlPanelInfo.DescriptorType);

        return services;
    }

    internal static IServiceCollection AddControlPanelState(this IServiceCollection services,
        ControlPanelInfo controlPanelInfo)
            => services.AddKeyedScoped(controlPanelInfo.StateType, controlPanelInfo.KeyedServiceKey);

    internal static IServiceCollection AddControlPanelCategoryDescriptor(this IServiceCollection services,
        ControlPanelInfo controlPanelInfo)
    {
        if (controlPanelInfo.CategoryDescriptorType is not null)
            services.AddScoped(controlPanelInfo.CategoryDescriptorType);

        return services;
    }

    internal static IServiceCollection AddControlPanelGroupDescriptor(this IServiceCollection services,
        ControlPanelInfo controlPanelInfo)
    {
        if (controlPanelInfo.GroupDescriptorType is not null)
            services.AddScoped(controlPanelInfo.GroupDescriptorType);

        return services;
    }

    private static IServiceCollection AddControlPanelRegistry<TClientModule>(this IServiceCollection services)
        where TClientModule : class, IClientModule
    {
        var serviceCountBefore = services.Count;

        services.TryAddScoped(serviceProvider =>
        {
            var registryFactory = serviceProvider.GetRequiredService<IControlPanelRegistryFactory>();
            var registry = registryFactory.CreateControlPanelRegistry<TClientModule>();

            Func<IControlPanelDescriptor, IControlPanelState, IControlPanelCategoryDescriptor, IControlPanelGroupDescriptor?, IAuthorizationRequirement?, IControlPanelRegistryItem> addMethodDelegate =
                registry.Add<ControlPanelBase<IControlPanelState>, IControlPanelState>;

            var addMethodInfo = addMethodDelegate.Method.GetGenericMethodDefinition();

            var controlPanelInfos = serviceProvider.GetKeyedServices<ControlPanelInfo>(typeof(TClientModule));
            foreach (var i in controlPanelInfos)
            {
                var descriptorType = typeof(IControlPanelDescriptor<>).MakeGenericType(i.ComponentType);

                if (serviceProvider.GetKeyedServices(descriptorType, i.KeyedServiceKey).FirstOrDefault() is not IControlPanelDescriptor descriptor)
                    continue;

                var state = (IControlPanelState)serviceProvider.GetRequiredKeyedService(i.StateType, i.KeyedServiceKey);

                IControlPanelCategoryDescriptor? categoryDescriptor = null;
                if (i.CategoryDescriptorType is not null)
                    categoryDescriptor = serviceProvider.GetService(i.CategoryDescriptorType) as IControlPanelCategoryDescriptor;

                IControlPanelGroupDescriptor? groupDescriptor = null;
                if (i.GroupDescriptorType is not null)
                    groupDescriptor = serviceProvider.GetService(i.GroupDescriptorType) as IControlPanelGroupDescriptor;

                if (categoryDescriptor is not null)
                {
                    var authorizationRequirement = i.ModuleAuthorizeAttribute.GetAccessLevelRequirement();

                    addMethodInfo.MakeGenericMethod(i.ComponentType, i.StateType).Invoke(registry, [descriptor, state, categoryDescriptor, groupDescriptor, authorizationRequirement]);
                }
            }

            return registry;
        });

        if (serviceCountBefore < services.Count)
        {
            // additionally register base interface to allow resolving all registries into an IEnumerable<>
            services.AddScoped<IControlPanelRegistry>(
                serviceProvider => serviceProvider.GetRequiredService<IControlPanelRegistry<TClientModule>>());
        }

        return services;
    }
}
