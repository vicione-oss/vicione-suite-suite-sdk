namespace Sdk.Client.Components.Cards.Contracts;

/// <summary>
/// Defines the data model for a card component.
/// </summary>
public interface ICardModel
{
    /// <summary>
    /// Gets the unique identifier of the card model.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets or sets the main title of the card.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the short teaser text displayed on the card.
    /// </summary>
    string TeaserText { get; set; }

    /// <summary>
    /// Gets or sets the URL to the teaser image displayed on the card.
    /// </summary>
    Uri? TeaserImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the main text content of the card.
    /// </summary>
    string Text { get; set; }
}
