using Sdk.Connections.Contracts;

namespace Sdk.Client.Services;

/// <summary>
/// Provides access to the current set of connections and notifies subscribers when
/// the state of those connections changes.
/// </summary>
public interface IConnectionService
{
    /// <summary>
    /// Gets the current list of connections managed by the service.
    /// </summary>
    /// <remarks>
    /// This list is a snapshot of the current state.  
    /// To react to changes, subscribe to <see cref="ConnectionStateChanged"/>.
    /// </remarks>
    IReadOnlyList<Connection> Connections { get; }

    /// <summary>
    /// Occurs when the state of one or more connections has changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The event handler receives the new snapshot of <see cref="Connections"/> as an argument.
    /// </para>
    /// <para>
    /// This event uses an asynchronous pattern, allowing handlers to perform asynchronous work
    /// before the change notification completes.
    /// </para>
    /// </remarks>
    event Func<IReadOnlyList<Connection>, Task>? ConnectionStateChanged;
}
