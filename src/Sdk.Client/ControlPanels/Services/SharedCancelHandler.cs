namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Cancels the changes of the control panels sharing an owner and <typeparamref name="TCancelHandler"/> with one
/// invocation of it, however many of them are canceled together.
/// </summary>
internal sealed class SharedCancelHandler<TState, TCancelHandler>(TCancelHandler cancelHandler, Type ownerServiceKey,
    SharedHandlerInvocations sharedHandlerInvocations)
    : IControlPanelCancelHandler<TState>, ISharedControlPanelHandler
    where TState : IControlPanelState
    where TCancelHandler : class, IControlPanelCancelHandler<TState>
{
    public Type OwnerServiceKey => ownerServiceKey;

    public Type HandlerType => typeof(TCancelHandler);

    public Task Cancel(TState state, CancellationToken cancellationToken)
        => sharedHandlerInvocations.InvokeOrJoin(ownerServiceKey, typeof(IControlPanelCancelHandler<TState>), typeof(TCancelHandler),
            async invocationCancellationToken =>
            {
                await cancelHandler.Cancel(state, invocationCancellationToken);

                return true;
            }, cancellationToken);
}
