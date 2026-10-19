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
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the control panel services of <typeparamref name="TClientModule"/> and returns a builder that adds
        /// <typeparamref name="TControlPanel"/> to them, e.g. with <c>WithAutoDiscovery</c>.
        /// </summary>
        public IControlPanelBuilder<TClientModule, TControlPanel, TState> AddControlPanel<TClientModule, TControlPanel, TState>()
            where TClientModule : class, IClientModule
            where TControlPanel : ControlPanelBase<TState>
            where TState : class, IControlPanelState
        {
            services.AddControlPanelCore<TClientModule>();

            var builder = new ControlPanelBuilder<TClientModule, TControlPanel, TState>(services);

            return builder;
        }

        /// <summary>
        /// Registers <see cref="IControlPanelRegistry{TClientModule}"/>, and <see cref="IControlPanelRegistry"/> for it, once per module.
        /// </summary>
        public IServiceCollection AddControlPanelCore<TClientModule>()
            where TClientModule : class, IClientModule
        {
            services.AddControlPanelRegistry<TClientModule>();

            return services;
        }

        internal IServiceCollection AddControlPanelDescriptor(ControlPanelInfo controlPanelInfo)
        {
            var descriptorInterfaceType = typeof(IControlPanelDescriptor<>).MakeGenericType(controlPanelInfo.ComponentType);

            if (descriptorInterfaceType.IsAssignableFrom(controlPanelInfo.DescriptorType))
                services.AddKeyedScoped(descriptorInterfaceType, controlPanelInfo.ServiceKey, controlPanelInfo.DescriptorType);

            return services;
        }

        // Without replacing a state registered for the control panel already, e.g. the one it shares with another.
        internal IServiceCollection AddControlPanelState(ControlPanelInfo controlPanelInfo)
        {
            services.TryAddKeyedScoped(controlPanelInfo.StateType, controlPanelInfo.ServiceKey);

            return services;
        }

        internal IServiceCollection AddControlPanelCategoryDescriptor(ControlPanelInfo controlPanelInfo)
        {
            // An interface cannot be registered without its implementation type, so its implementor registers it.
            if (controlPanelInfo.CategoryDescriptorType?.IsInterface == true)
                return services;

            if (controlPanelInfo.CategoryDescriptorType is not null)
                services.AddScoped(controlPanelInfo.CategoryDescriptorType);

            return services;
        }

        internal IServiceCollection AddControlPanelGroupDescriptor(ControlPanelInfo controlPanelInfo)
        {
            if (controlPanelInfo.GroupDescriptorType is not null)
                services.AddScoped(controlPanelInfo.GroupDescriptorType);

            return services;
        }

        // registers the parent descriptor types a control panel names - as a parent, as a child, or both - so
        // that either side of a link can resolve the descriptor without the other having been registered at all
        internal IServiceCollection AddControlPanelParentDescriptor(ControlPanelInfo controlPanelInfo)
        {
            AddParentDescriptorType(controlPanelInfo.ControlPanelParentAttribute?.ParentDescriptorType);
            AddParentDescriptorType(controlPanelInfo.ControlPanelChildAttribute?.ParentDescriptorType);

            return services;

            void AddParentDescriptorType(Type? parentDescriptorType)
            {
                // as for the category descriptor, an interface is assumed to be registered by its implementor
                if (parentDescriptorType is null or { IsInterface: true })
                    return;

                // registered under its own type even when it is also a control panel's descriptor, which is
                // only registered as a keyed IControlPanelDescriptor<TControlPanel> the other side cannot ask for
                services.TryAddScoped(parentDescriptorType);
            }
        }

        private IServiceCollection AddControlPanelRegistry<TClientModule>()
            where TClientModule : class, IClientModule
        {
            var serviceCountBefore = services.Count;

            services.TryAddScoped(serviceProvider =>
            {
                var registryFactory = serviceProvider.GetRequiredService<IControlPanelRegistryFactory>();
                var registry = registryFactory.CreateControlPanelRegistry<TClientModule>();

                Func<IControlPanelDescriptor, IControlPanelState, IControlPanelCategoryDescriptor, IControlPanelGroupDescriptor?, IAuthorizationRequirement?, IControlPanelParentDescriptor?, IControlPanelParentDescriptor?, IControlPanelRegistryItem> addMethodDelegate =
                    registry.Add<ControlPanelBase<IControlPanelState>, IControlPanelState>;

                var addMethodInfo = addMethodDelegate.Method.GetGenericMethodDefinition();

                var controlPanelInfos = serviceProvider.GetKeyedServices<ControlPanelInfo>(typeof(TClientModule));
                foreach (var i in controlPanelInfos)
                {
                    var descriptorType = typeof(IControlPanelDescriptor<>).MakeGenericType(i.ComponentType);

                    if (serviceProvider.GetKeyedServices(descriptorType, i.ServiceKey).FirstOrDefault() is not IControlPanelDescriptor descriptor)
                        continue;

                    var state = (IControlPanelState)serviceProvider.GetRequiredKeyedService(i.StateType, i.ServiceKey);

                    IControlPanelCategoryDescriptor? categoryDescriptor = null;
                    if (i.CategoryDescriptorType is not null)
                        categoryDescriptor = serviceProvider.GetService(i.CategoryDescriptorType) as IControlPanelCategoryDescriptor;

                    IControlPanelGroupDescriptor? groupDescriptor = null;
                    if (i.GroupDescriptorType is not null)
                        groupDescriptor = serviceProvider.GetService(i.GroupDescriptorType) as IControlPanelGroupDescriptor;

                    if (categoryDescriptor is not null)
                    {
                        var authorizationRequirement = i.ModuleAuthorizeAttribute.GetAccessLevelAuthorizationRequirement();

                        var childOf = i.ControlPanelChildAttribute?.ParentDescriptorType.Resolve<IControlPanelParentDescriptor>(serviceProvider);
                        var asParent = i.ControlPanelParentAttribute?.ParentDescriptorType.Resolve<IControlPanelParentDescriptor>(serviceProvider);

                        addMethodInfo.MakeGenericMethod(i.ComponentType, i.StateType).Invoke(registry, [descriptor, state, categoryDescriptor, groupDescriptor, authorizationRequirement, childOf, asParent]);
                    }
                }

                return registry;
            });

            if (serviceCountBefore < services.Count)
            {
                // The non-generic interface lets every module's registry resolve through one IEnumerable<IControlPanelRegistry>.
                services.AddScoped<IControlPanelRegistry>(
                    serviceProvider => serviceProvider.GetRequiredService<IControlPanelRegistry<TClientModule>>());
            }

            return services;
        }
    }
}
