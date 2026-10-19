using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <summary>
/// Refers to the state of a control panel that other control panels share instead of each having a state of their own,
/// created with <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}.WithSharedState(out ISharedControlPanelState{TState})"/>
/// and passed to <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}.WithSharedState(ISharedControlPanelState{TState})"/>
/// of each of these control panels.
/// </summary>
/// <remarks>
/// It is not a state itself; the control panels resolve the state the owning control panel has in the current scope.
/// </remarks>
public interface ISharedControlPanelState<TState>
    where TState : IControlPanelState;
