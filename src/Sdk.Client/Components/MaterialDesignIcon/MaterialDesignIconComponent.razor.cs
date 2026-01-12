using Microsoft.AspNetCore.Components;
using Sdk.Client.Enums;

namespace Sdk.Client.Components.MaterialDesignIcon;

public sealed partial class MaterialDesignIconComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public MaterialDesignIconName Name { get; set; }
}
