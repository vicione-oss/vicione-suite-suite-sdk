using Bunit;
using AwesomeAssertions;
using Sdk.Client.Components.Cards.Contracts;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Client.Tests.Components.Cards;

public sealed class CardTests
{
    public class OnCloseButtonClick
    {
        [Fact]
        public void Invokes_event()
        {
            using var ctx = new BunitContext();
            var card = new Card
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                TeaserText = "Teaser",
            };
            var invoked = false;
            ctx.SetupSuiteServices();

            var component = ctx.Render<Client.Components.Cards.Components.Card>(b =>
            {
                b.Add(p => p.CardModel, card);
                b.Add(p => p.OnCloseClick, () => invoked = true);
            });

            component.Find(".close").Click();

            invoked.Should().BeTrue();
        }

        [Fact]
        public void Invokes_event_when_card_has_image()
        {
            using var ctx = new BunitContext();
            var card = new Card
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                TeaserText = "Teaser",
                TeaserImageUrl = new Uri("https://example.com/image.jpg")
            };
            var invoked = false;
            ctx.SetupSuiteServices();

            var component = ctx.Render<Client.Components.Cards.Components.Card>(b =>
            {
                b.Add(p => p.CardModel, card);
                b.Add(p => p.OnCloseClick, () => invoked = true);
            });

            component.Find(".close").Click();

            invoked.Should().BeTrue();
        }
    }

    public class OnLinkClick
    {
        [Fact]
        public void Invokes_event()
        {
            using var ctx = new BunitContext();
            var card = new Card
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                TeaserText = "Teaser",
                Text = "Text"
            };
            var invoked = false;

            ctx.SetupSuiteServices();

            var component = ctx.Render<Client.Components.Cards.Components.Card>(b =>
            {
                b.Add(p => p.CardModel, card);
                b.Add(p => p.OnLinkClick, () => invoked = true);
            });

            var divElementText = component.Find(".text");
            var buttonElement = divElementText.NextElementSibling;   // next element is the 'LinkButton'
            buttonElement?.Click();

            invoked.Should().BeTrue();
        }
    }

    private sealed class Card : ICardModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TeaserText { get; set; } = string.Empty;
        public Uri? TeaserImageUrl { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
