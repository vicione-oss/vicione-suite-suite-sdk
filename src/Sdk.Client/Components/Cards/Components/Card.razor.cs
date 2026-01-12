using Microsoft.AspNetCore.Components;
using Sdk.Client.Components.Cards.Contracts;

namespace Sdk.Client.Components.Cards.Components;

public sealed partial class Card
{
    [Parameter]
    public required ICardModel CardModel { get; set; }

    [Parameter]
    public EventCallback<ICardModel> OnLinkClick { get; set; }

    [Parameter]
    public EventCallback<ICardModel> OnCloseClick { get; set; }
}
