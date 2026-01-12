using Sdk.Client.Infrastructure;

namespace Sdk.Testing.Client;

/// <summary>
/// A factory class for creating <see cref="ClientContext{TMessage}"/> instances for testing.
/// </summary>
public static class ClientContextFactory
{
    /// <summary>
    /// Creates a new <see cref="ClientContext{TMessage}"/> with the specified <paramref name="message"/>
    /// and an optional <paramref name="correlationId"/>.
    /// </summary>
    public static ClientContext<TMessage> Create<TMessage>(TMessage message, Guid correlationId = new Guid())
        where TMessage : class
            => new(message, correlationId);
}
