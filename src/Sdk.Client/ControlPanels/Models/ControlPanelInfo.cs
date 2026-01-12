using Sdk.Authorization;
using Sdk.Client.ControlPanels.Services;

namespace Sdk.Client.ControlPanels.Models;

/// <summary>
/// Holds information required to register services related to a control panel in DI container and 
/// to orchestrate calls to <see cref="IControlPanelRegistry{TClientModule}.Add{TComponent, TState}(IControlPanelDescriptor, TState, IControlPanelCategoryDescriptor, IControlPanelGroupDescriptor?, Microsoft.AspNetCore.Authorization.IAuthorizationRequirement?)"/>
/// </summary>
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
