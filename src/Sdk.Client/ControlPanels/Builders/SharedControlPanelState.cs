using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Builders;

/// <inheritdoc cref="ISharedControlPanelState{TState}"/>
internal sealed class SharedControlPanelState<TState> : ISharedControlPanelState<TState>
    where TState : IControlPanelState
{
    /// <summary>
    /// Gets the <see cref="ControlPanelServiceKey{TControlPanel}"/> type of the control panel whose state is shared.
    /// </summary>
    internal Type OwnerServiceKey { get; }

    internal SharedControlPanelState(Type ownerServiceKey)
        => OwnerServiceKey = ownerServiceKey;
}
