using Microsoft.AspNetCore.Components;
using Sdk.Connections.Contracts;

namespace Sdk.Client.Connections;

/// <summary>
/// Provides a base class for components that render settings for a specific connection type.
/// </summary>
public class ConnectionSettingsComponentBase<TConnection> : ComponentBase
    where TConnection : IConnection
{
    /// <summary>
    /// Gets or sets an event callback that is invoked when a setting has changed.
    /// </summary>
    [Parameter]
    public EventCallback Changed { get; set; }

    /// <summary>
    /// Gets or sets the connection data model to be displayed and edited.
    /// </summary>
    [Parameter, EditorRequired]
    public TConnection Connection { get; set; }

    /// <summary>
    /// Invokes the <see cref="Changed"/> event callback to notify consumers that a change has occurred.
    /// This should be called by derived classes whenever a setting is modified.
    /// </summary>
    protected async Task NotifyChanged()
    {
        if (Changed.HasDelegate)
            await Changed.InvokeAsync();
    }
}
