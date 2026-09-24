using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Authorization;
using Sdk.Authorization.Extensions;
using Sdk.Client.Extensions;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Attributes;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.NotificationArea.Services;

namespace Sdk.Client.NotificationArea.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register services related to the notification area.
/// </summary>
public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddNotificationElementStates<TClientModule>(IEnumerable<NotificationElementInfo<TClientModule>> notificationElementInfos)
            where TClientModule : class, IClientModule
        {
            foreach (var notificationElementInfo in notificationElementInfos)
                services.AddKeyedScoped(notificationElementInfo.StateType, notificationElementInfo.StateServiceKey);

            return services;
        }

        private IServiceCollection AddNotificationElementRegistry<TClientModule>(IEnumerable<NotificationElementInfo<TClientModule>> notificationElementInfos)
            where TClientModule : class, IClientModule
        {
            services.AddScoped(serviceProvider =>
            {
                var registryFactory = serviceProvider.GetRequiredService<INotificationElementRegistryFactory>();
                var registry = registryFactory.CreateNotificationElementRegistry<TClientModule>();

                var registryType = registry.GetType();

                const string MethodName = nameof(registry.Add);
                var methodInfo = registryType.GetMethod(MethodName);
                if (methodInfo is not null)
                {
                    foreach (var i in notificationElementInfos)
                    {
                        var initialNotificationElementAttribute = i.InitialNotificationElementAttribute;
                        var position = initialNotificationElementAttribute.Position;
                        var id = Guid.Parse(initialNotificationElementAttribute.Id);

                        var state = (INotificationElementState)serviceProvider.GetRequiredKeyedService(i.StateType, i.StateServiceKey);
                        state.Visible = initialNotificationElementAttribute.Visible;

                        var authorizationRequirement = i.ModuleAuthorizeAttribute.GetAccessLevelAuthorizationRequirement();

                        methodInfo.MakeGenericMethod(i.ComponentType, i.StateType).Invoke(registry, [state, position, id, authorizationRequirement]);
                    }

                    return registry;
                }
                else
                {
                    throw new MethodAccessException($"Could not find '{registryType.Name}.{MethodName}()'");
                }
            });

            // The non-generic interface lets every module's registry resolve through one IEnumerable<INotificationElementRegistry>.
            services.AddScoped<INotificationElementRegistry>(
                serviceProvider => serviceProvider.GetRequiredService<INotificationElementRegistry<TClientModule>>());

            return services;
        }

        /// <summary>
        /// Registers every public <see cref="NotificationElementBase{TState}"/> in the assembly of <typeparamref name="TClientModule"/>
        /// that carries <see cref="InitialNotificationElementAttribute{TClientModule}"/>, and the module's
        /// <see cref="INotificationElementRegistry{TClientModule}"/>.
        /// </summary>
        /// <remarks>
        /// Each element's state type is registered as a keyed service under
        /// <see cref="NotificationElementServiceKey{TClientModule, TNotificationElement}"/>.
        /// </remarks>
        public IServiceCollection AddNotificationElements<TClientModule>()
            where TClientModule : class, IClientModule
        {
            var notificationElementInfos = GetNotificationElementInfos<TClientModule>();

            services
                .AddNotificationElementStates(notificationElementInfos)
                .AddNotificationElementRegistry(notificationElementInfos);

            return services;
        }
    }

    private static List<NotificationElementInfo<TClientModule>> GetNotificationElementInfos<TClientModule>()
        where TClientModule : class, IClientModule
    {
        var clientModuleTypes = typeof(TClientModule).Assembly.GetExportedTypes();

        var notificationElementInfos = clientModuleTypes
            .Where(t => Attribute.IsDefined(t, typeof(InitialNotificationElementAttribute<TClientModule>)))
            .Select(t => new
            {
                NotificationElementComponentType = t,
                NotificationElementBaseType = t.GetBaseTypeRecursive(typeof(NotificationElementBase<>))
            })
            .Select(i => new NotificationElementInfo<TClientModule>
            {
                ComponentType = i.NotificationElementComponentType,
                StateType = GetNotificationElementStateType(i.NotificationElementBaseType),
                StateServiceKey = typeof(NotificationElementServiceKey<,>).MakeGenericType(typeof(TClientModule), i.NotificationElementComponentType),
                InitialNotificationElementAttribute = i.NotificationElementComponentType.GetCustomAttribute<InitialNotificationElementAttribute<TClientModule>>()!,
                ModuleAuthorizeAttribute = i.NotificationElementComponentType.GetCustomAttribute<ModuleAuthorizeAttribute>()
            })
            .ToList();

        return notificationElementInfos;
    }

    private static Type GetNotificationElementStateType(Type notificationElementBaseType)
    {
        var notificationElementStateType = notificationElementBaseType.GenericTypeArguments[0];

        if (!typeof(INotificationElementState).IsAssignableFrom(notificationElementStateType))
            throw new InvalidOperationException($"First generic type argument of '{notificationElementStateType.Name}' is not assignable to '{nameof(INotificationElementState)}'");

        if (!notificationElementStateType.IsClass)
            throw new InvalidOperationException($"First generic type argument of '{notificationElementStateType.Name}' is not a class");

        return notificationElementStateType;
    }

    private sealed class NotificationElementInfo<TClientModule>
        where TClientModule : class, IClientModule
    {
        public required Type ComponentType { get; init; }
        public required Type StateType { get; init; }
        public required Type StateServiceKey { get; init; }
        public required InitialNotificationElementAttribute<TClientModule> InitialNotificationElementAttribute { get; init; }
        public required ModuleAuthorizeAttribute? ModuleAuthorizeAttribute { get; init; }
    }
}
