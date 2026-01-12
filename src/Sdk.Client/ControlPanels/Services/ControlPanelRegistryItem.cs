using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

internal sealed record ControlPanelRegistryItem<TClientModule>(Type ComponentType, IControlPanelDescriptor Descriptor,
    IControlPanelState State, IControlPanelCategoryDescriptor CategoryDescriptor, IControlPanelGroupDescriptor GroupDescriptor,
    IAuthorizationRequirement? AuthorizationRequirement) :
        IControlPanelRegistryItem<TClientModule>
            where TClientModule : class, IClientModule;
