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
            KeyedServiceKey = typeof(ControlPanelServiceKey<>).MakeGenericType(componentType),
            ModuleAuthorizeAttribute = componentType.GetCustomAttribute<ModuleAuthorizeAttribute>()
        };

        services.AddKeyedScoped(typeof(TClientModule), (_, __) => controlPanelInfo);

        services
            .AddControlPanelDescriptor(controlPanelInfo)
            .AddControlPanelState(controlPanelInfo)
            .AddControlPanelCategoryDescriptor(controlPanelInfo)
            .AddControlPanelGroupDescriptor(controlPanelInfo);

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IControlPanelSaveHandler<TState>
    {
        services.TryAddScoped<IControlPanelSaveHandler<TState>, TSaveHandler>();

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IControlPanelCancelHandler<TState>
    {
        services.TryAddScoped<IControlPanelCancelHandler<TState>, TCancelHandler>();

        return this;
    }

    public IControlPanelBuilder<TClientModule, TControlPanel, TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IControlPanelResetHandler<TState>
    {
        services.TryAddScoped<IControlPanelResetHandler<TState>, TResetHandler>();

        return this;
    }
}
