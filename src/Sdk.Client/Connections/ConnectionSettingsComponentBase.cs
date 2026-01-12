using Microsoft.AspNetCore.Components;
using Sdk.Connections.Contracts;

namespace Sdk.Client.Connections;

public class ConnectionSettingsComponentBase<TConnection> : ComponentBase
    where TConnection : IConnection
{
    [Parameter]
    public EventCallback Changed { get; set; }

    [Parameter, EditorRequired]
    public TConnection Connection { get; set; }

    protected async Task NotifyChanged()
    {
        if (Changed.HasDelegate)
            await Changed.InvokeAsync();
    }
}
