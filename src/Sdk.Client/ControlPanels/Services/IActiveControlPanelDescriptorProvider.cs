namespace Sdk.Client.ControlPanels.Services;

public interface IActiveControlPanelDescriptorProvider
{
    IControlPanelDescriptor? GetActiveControlPanelDescriptor();
}
