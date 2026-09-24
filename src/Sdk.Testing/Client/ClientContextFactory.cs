using Sdk.Client.Infrastructure;

namespace Sdk.Testing.Client;

/// <summary>
/// A factory class for creating <see cref="ClientContext{TMessage}"/> instances for testing.
/// </summary>
public static class ClientContextFactory
{
    /// <summary>
    /// Wraps <paramref name="message"/> in a <see cref="ClientContext{TMessage}"/>; <paramref name="correlationId"/> defaults to
    /// <see cref="Guid.Empty"/>.
    /// </summary>
    public static ClientContext<TMessage> Create<TMessage>(TMessage message, Guid correlationId = new Guid())
        where TMessage : class
            => new(message, correlationId);
}
