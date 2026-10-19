using Sdk.Client.ControlPanels.Models;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Saves the control panels sharing an owner and <typeparamref name="TSaveHandler"/> with one invocation of it, however
/// many of them are saved together, so a change on several tabs neither saves the entity twice nor, for a new one,
/// creates it twice.
/// </summary>
internal sealed class SharedSaveHandler<TState, TSaveHandler>(TSaveHandler saveHandler, Type ownerServiceKey,
    SharedHandlerInvocations sharedHandlerInvocations)
    : IControlPanelSaveHandler<TState>, ISharedControlPanelHandler
    where TState : IControlPanelState
    where TSaveHandler : class, IControlPanelSaveHandler<TState>
{
    public Type OwnerServiceKey => ownerServiceKey;

    public Type HandlerType => typeof(TSaveHandler);

    public Task<ISaveResult> Save(TState state, CancellationToken cancellationToken)
        => sharedHandlerInvocations.InvokeOrJoin(ownerServiceKey, typeof(IControlPanelSaveHandler<TState>), typeof(TSaveHandler),
            invocationCancellationToken => saveHandler.Save(state, invocationCancellationToken), cancellationToken);
}
