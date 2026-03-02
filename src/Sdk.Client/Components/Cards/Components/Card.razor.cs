using Sdk.Client.Components.Cards.Contracts;

namespace Sdk.Client.Components.Cards.Components;

/// <summary>
/// A component that displays content in a card-like format.
/// </summary>
public sealed partial class Card
{
    /// <summary>
    /// Gets or sets the data model that defines the content of the card.
    /// </summary>
    [Parameter]
    public required ICardModel CardModel { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the link of the card is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<ICardModel> OnLinkClick { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the card's close button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<ICardModel> OnCloseClick { get; set; }
}
