using Sdk.Client.Infrastructure;

namespace Sdk.Testing.Client;

public static class ClientContextFactory
{
    public static ClientContext<TMessage> Create<TMessage>(TMessage message, Guid correlationId = new Guid()) where TMessage : class
        => new(message, correlationId);
}
