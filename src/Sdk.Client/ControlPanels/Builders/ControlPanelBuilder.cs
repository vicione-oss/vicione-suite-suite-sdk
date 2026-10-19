using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Authorization;
using Sdk.Client.ControlPanels.Components;
using Sdk.Client.ControlPanels.Extensions;
using Sdk.Client.ControlPanels.Models;
using Sdk.Client.ControlPanels.Services;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Builders;

internal sealed class ControlPanelBuilder<TClientModule, TControlPanel, TState>(IServiceCollection services)
    : IControlPanelBuilder<TClientModule, TControlPanel, TState>
        where TClientModule : class, IClientModule
        where TControlPanel : class, IControlPanel
        where TState : IControlPanelState
{
    /// <summary>
    /// The key under which the services of <typeparamref name="TControlPanel"/> are registered: its state, its handlers and the
    /// state and handlers it shares with other control panels.
    /// </summary>
    private static Type ServiceKey => typeof(ControlPanelServiceKey<>).MakeGenericType(typeof(TControlPanel));

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithAutoDiscovery<TDescriptor>()
        where TDescriptor : class, IControlPanelDescriptor<TControlPanel>
    {
        var componentType = typeof(TControlPanel);

        var controlPanelInfo = new ControlPanelInfo
        {
            ComponentType = componentType,
            StateType = typeof(TState),
            DescriptorType = typeof(TDescriptor),
            CategoryDescriptorType = componentType.GetControlPanelCategoryDescriptorType(),
            GroupDescriptorType = componentType.GetControlPanelGroupDescriptorType(),
            ServiceKey = ServiceKey,
            ControlPanelParentAttribute = componentType.GetControlPanelParentAttribute(),
            ControlPanelChildAttribute = componentType.GetControlPanelChildAttribute(),
            ModuleAuthorizeAttribute = componentType.GetCustomAttribute<ModuleAuthorizeAttribute>()
        };

        services.AddKeyedScoped(typeof(TClientModule), (_, __) => controlPanelInfo);

        services
            .AddControlPanelDescriptor(controlPanelInfo)
            .AddControlPanelState(controlPanelInfo)
            .AddControlPanelCategoryDescriptor(controlPanelInfo)
            .AddControlPanelGroupDescriptor(controlPanelInfo)
            .AddControlPanelParentDescriptor(controlPanelInfo);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IControlPanelSaveHandler<TState>
    {
        services.TryAddKeyedScoped<IControlPanelSaveHandler<TState>, TSaveHandler>(ServiceKey);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IControlPanelCancelHandler<TState>
    {
        services.TryAddKeyedScoped<IControlPanelCancelHandler<TState>, TCancelHandler>(ServiceKey);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IControlPanelResetHandler<TState>
    {
        services.TryAddKeyedScoped<IControlPanelResetHandler<TState>, TResetHandler>(ServiceKey);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedHandlers(
        Action<ISharedControlPanelHandlersBuilder<TState>> configureSharedHandlers,
        out ISharedControlPanelHandlers<TState> sharedHandlers)
    {
        var sharedHandlersBuilder = new SharedControlPanelHandlersBuilder<TState>(services, typeof(TControlPanel), ServiceKey);

        configureSharedHandlers(sharedHandlersBuilder);

        sharedHandlers = sharedHandlersBuilder.Build();

        return WithSharedHandlers(sharedHandlers);
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedHandlers(ISharedControlPanelHandlers<TState> sharedHandlers)
    {
        var sdkSharedHandlers = sharedHandlers as SharedControlPanelHandlers<TState>
            ?? throw new NotSupportedException($"The shared handlers are not ones {nameof(WithSharedHandlers)} created.");

        sdkSharedHandlers.AddTo(services, ServiceKey);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedState(out ISharedControlPanelState<TState> sharedState)
    {
        // Registered here as well, so the state can be shared before or without WithAutoDiscovery.
        services.TryAddKeyedScoped(typeof(TState), ServiceKey);

        sharedState = new SharedControlPanelState<TState>(ServiceKey);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSharedState(ISharedControlPanelState<TState> sharedState)
    {
        var ownerServiceKey = (sharedState as SharedControlPanelState<TState>
            ?? throw new NotSupportedException($"The shared state is not one {nameof(WithSharedState)} created.")).OwnerServiceKey;

        // Resolving the state would resolve itself endlessly, ending in a StackOverflowException that takes the process down.
        if (ownerServiceKey == ServiceKey)
            throw new InvalidOperationException($"'{typeof(TControlPanel)}' cannot be given the state it shares itself.");

        for (var i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].IsKeyedService && services[i].ServiceType == typeof(TState) && Equals(services[i].ServiceKey, ServiceKey))
                services.RemoveAt(i);
        }

        services.AddKeyedScoped(typeof(TState), ServiceKey,
            (serviceProvider, _) => serviceProvider.GetRequiredKeyedService(typeof(TState), ownerServiceKey));

        return this;
    }
}
