using Sdk.Authorization;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// Holds what is needed to register a control panel's services and later add it to its
/// <see cref="IControlPanelRegistry{TClientModule}"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ControlPanelInfo
{
    public required Type ComponentType { get; init; }
    public required Type StateType { get; init; }
    public Type? DescriptorType { get; set; }
    public Type? CategoryDescriptorType { get; set; }
    public Type? GroupDescriptorType { get; set; }
    public required Type KeyedServiceKey { get; init; }
    public ModuleAuthorizeAttribute? ModuleAuthorizeAttribute { get; init; }
}
