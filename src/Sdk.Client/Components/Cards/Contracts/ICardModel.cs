namespace Sdk.Client.Components.Cards.Contracts;

public interface ICardModel
{
    public Guid Id { get; }
    public string Title { get; set; }
    public string TeaserText { get; set; }
    public string TeaserImagePath { get; set; }
    public string Text { get; set; }
}
