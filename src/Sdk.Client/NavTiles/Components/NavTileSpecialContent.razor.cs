using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NavTiles.Components;

public partial class NavTileSpecialContent : ComponentBase
{
    [Parameter, EditorRequired] public string Headline { get; set; }
    [Parameter] public string? Subline { get; set; }
    [Parameter] public RenderFragment? LeftContent { get; set; }
    [Parameter] public string? LeftContentDescription { get; set; }
    [Parameter] public RenderFragment? RightContent { get; set; }
    [Parameter] public string? RightContentDescription { get; set; }
}
