using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Cards.Components;

public sealed partial class CardList
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
