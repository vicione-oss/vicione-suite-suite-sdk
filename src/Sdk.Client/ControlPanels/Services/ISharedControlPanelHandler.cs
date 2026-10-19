using Sdk.Client.ControlPanels.Builders;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// A save, cancel or reset handler that control panels editing one entity together share, e.g. a parent and its tabs,
/// configured with <see cref="ISharedControlPanelHandlers{TState}"/>.
/// </summary>
/// <remarks>
/// A host that keeps an edit per control panel tells the control panels sharing a handler by <see cref="OwnerServiceKey"/>
/// and <see cref="HandlerType"/>, e.g. to reset them once when they are shown rather than once per control panel.
/// </remarks>
public interface ISharedControlPanelHandler
{
    /// <summary>
    /// Gets the <see cref="ControlPanelServiceKey{TControlPanel}"/> type of the control panel owning the entity, which created
    /// the <see cref="ISharedControlPanelHandlers{TState}"/> the control panels sharing the handler were configured with.
    /// </summary>
    Type OwnerServiceKey { get; }

    /// <summary>
    /// Gets the handler the control panels share, which runs once for all of them per save, cancel or reset.
    /// </summary>
    Type HandlerType { get; }
}
