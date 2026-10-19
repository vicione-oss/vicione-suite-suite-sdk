using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <summary>
/// The handlers a control panel shares with the other control panels editing its entity, e.g. its tabs, created with
/// <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}.WithSharedHandlers(Action{ISharedControlPanelHandlersBuilder{TState}}, out ISharedControlPanelHandlers{TState})"/>
/// and passed to <see cref="IControlPanelBuilder{TClientModule, TControlPanel, TState}.WithSharedHandlers(ISharedControlPanelHandlers{TState})"/>
/// of each of these control panels.
/// </summary>
public interface ISharedControlPanelHandlers<TState>
    where TState : IControlPanelState;
