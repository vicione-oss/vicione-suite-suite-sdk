using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <summary>
/// Registers the shared handlers keyed by the owning control panel, once for all control panels sharing them, and collects
/// the registrations of the handler each of these control panels resolves, created from the owner's handler.
/// </summary>
internal sealed class SharedControlPanelHandlersBuilder<TState>(IServiceCollection services, Type ownerControlPanelType,
    Type ownerServiceKey)
    : ISharedControlPanelHandlersBuilder<TState>
    where TState : IControlPanelState
{
    private readonly List<Action<IServiceCollection, Type>> _registrations = [];

    public ISharedControlPanelHandlersBuilder<TState> WithSaveHandler<TSaveHandler>()
        where TSaveHandler : class, IControlPanelSaveHandler<TState>
    {
        AddSharedHandler<IControlPanelSaveHandler<TState>, TSaveHandler>("save");

        _registrations.Add((controlPanelServices, controlPanelServiceKey) =>
            controlPanelServices.TryAddKeyedScoped<IControlPanelSaveHandler<TState>>(controlPanelServiceKey, (serviceProvider, _)
                => new SharedSaveHandler<TState, TSaveHandler>(serviceProvider.GetRequiredKeyedService<TSaveHandler>(ownerServiceKey),
                    ownerServiceKey, serviceProvider.GetRequiredService<SharedHandlerInvocations>())));

        return this;
    }

    public ISharedControlPanelHandlersBuilder<TState> WithCancelHandler<TCancelHandler>()
        where TCancelHandler : class, IControlPanelCancelHandler<TState>
    {
        AddSharedHandler<IControlPanelCancelHandler<TState>, TCancelHandler>("cancel");

        _registrations.Add((controlPanelServices, controlPanelServiceKey) =>
            controlPanelServices.TryAddKeyedScoped<IControlPanelCancelHandler<TState>>(controlPanelServiceKey, (serviceProvider, _)
                => new SharedCancelHandler<TState, TCancelHandler>(serviceProvider.GetRequiredKeyedService<TCancelHandler>(ownerServiceKey),
                    ownerServiceKey, serviceProvider.GetRequiredService<SharedHandlerInvocations>())));

        return this;
    }

    public ISharedControlPanelHandlersBuilder<TState> WithResetHandler<TResetHandler>()
        where TResetHandler : class, IControlPanelResetHandler<TState>
    {
        AddSharedHandler<IControlPanelResetHandler<TState>, TResetHandler>("reset");

        _registrations.Add((controlPanelServices, controlPanelServiceKey) =>
            controlPanelServices.TryAddKeyedScoped<IControlPanelResetHandler<TState>>(controlPanelServiceKey, (serviceProvider, _)
                => new SharedResetHandler<TState, TResetHandler>(serviceProvider.GetRequiredKeyedService<TResetHandler>(ownerServiceKey),
                    ownerServiceKey, serviceProvider.GetRequiredService<SharedHandlerInvocations>())));

        return this;
    }

    public SharedControlPanelHandlers<TState> Build()
        => new([.. _registrations]);

    private void AddSharedHandler<THandlerService, THandler>(string operation)
        where THandler : class, THandlerService
    {
        // The owner would keep its own handler, as the shared one is only tried to add, and would not join the others sharing it.
        if (services.Any(d => d.IsKeyedService && Equals(d.ServiceKey, ownerServiceKey) && d.ServiceType == typeof(THandlerService)))
        {
            throw new InvalidOperationException($"'{ownerControlPanelType}' cannot share the {operation} handler "
                + $"'{typeof(THandler)}' as it has a {operation} handler of its own already.");
        }

        services.TryAddScoped<SharedHandlerInvocations>();
        services.TryAddKeyedScoped<THandler>(ownerServiceKey);
    }
}
