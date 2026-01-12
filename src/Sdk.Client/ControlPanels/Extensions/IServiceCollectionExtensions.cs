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

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Convenience method that calls <see cref="AddControlPanelCore{TClientModule}(IServiceCollection)"/> and
    /// returns a builder to configure advanced behavior of the given <typeparamref name="TControlPanel"/> with
    /// the given <typeparamref name="TState"/>.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add services to</param>
    /// <returns><see cref="IControlPanelBuilder{TControlPanel, TState}"/> for configuration of advanced behavior of <typeparamref name="TControlPanel"/></returns>
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
    /// Adds core services required for control panel functionality.
    /// </summary>
    /// <remarks>
    /// Registers <see cref="IControlPanelRegistry{TClientModule}"/> and <see cref="IControlPanelPageRegistry"/>
    /// in DI container when not already present.
    /// </remarks>
    public static IServiceCollection AddControlPanelCore<TClientModule>(this IServiceCollection services)
            where TClientModule : class, IClientModule
    {
        services.AddControlPanelRegistry<TClientModule>()
            .AddControlPanelPageRegistry();

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
        services.TryAddScoped<IDefaultControlPanelGroupDescriptor, DefaultControlPanelGroupDescriptor>();

        var serviceCountBefore = services.Count;

        services.TryAddScoped<IControlPanelRegistry<TClientModule>>(serviceProvider =>
        {
            var defaultControlPanelGroupDescriptor = serviceProvider.GetRequiredService<IDefaultControlPanelGroupDescriptor>();

            var registry = new ControlPanelRegistry<TClientModule>(defaultControlPanelGroupDescriptor);

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

    private static IServiceCollection AddControlPanelPageRegistry(this IServiceCollection services)
    {
        services.TryAddScoped<IControlPanelPageRegistry, ControlPanelPageRegistry>();

        return services;

    }
}
