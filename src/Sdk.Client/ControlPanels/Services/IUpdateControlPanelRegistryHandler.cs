namespace Sdk.Client.ControlPanels.Services;

public interface IUpdateControlPanelRegistryHandler
{
    Task Execute(CancellationToken cancellationToken);
}
