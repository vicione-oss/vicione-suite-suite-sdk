using Microsoft.Extensions.DependencyInjection;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <inheritdoc cref="ISharedControlPanelHandlers{TState}"/>
internal sealed class SharedControlPanelHandlers<TState> : ISharedControlPanelHandlers<TState>
    where TState : IControlPanelState
{
    private readonly IReadOnlyList<Action<IServiceCollection, Type>> _registrations;

    internal SharedControlPanelHandlers(IReadOnlyList<Action<IServiceCollection, Type>> registrations)
        => _registrations = registrations;

    /// <summary>
    /// Configures the shared handlers for the control panel keyed by <paramref name="controlPanelServiceKey"/>.
    /// </summary>
    internal void AddTo(IServiceCollection services, Type controlPanelServiceKey)
    {
        foreach (var registration in _registrations)
            registration(services, controlPanelServiceKey);
    }
}
