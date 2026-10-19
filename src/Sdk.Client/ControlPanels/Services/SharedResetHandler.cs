namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Resets the control panels sharing an owner and <typeparamref name="TResetHandler"/> with one invocation of it,
/// however many of them are reset together, e.g. after a cancel.
/// </summary>
internal sealed class SharedResetHandler<TState, TResetHandler>(TResetHandler resetHandler, Type ownerServiceKey,
    SharedHandlerInvocations sharedHandlerInvocations)
    : IControlPanelResetHandler<TState>, ISharedControlPanelHandler
    where TState : IControlPanelState
    where TResetHandler : class, IControlPanelResetHandler<TState>
{
    public Type OwnerServiceKey => ownerServiceKey;

    public Type HandlerType => typeof(TResetHandler);

    public Task Reset(TState state, CancellationToken cancellationToken)
        => sharedHandlerInvocations.InvokeOrJoin(ownerServiceKey, typeof(IControlPanelResetHandler<TState>), typeof(TResetHandler),
            async invocationCancellationToken =>
            {
                await resetHandler.Reset(state, invocationCancellationToken);

                return true;
            }, cancellationToken);
}
