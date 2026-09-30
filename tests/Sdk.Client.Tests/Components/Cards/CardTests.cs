using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Web;
using Sdk.Client.Components.Cards.Contracts;
using Sdk.Testing.Client;
using Xunit;

namespace Sdk.Client.Tests.Components.Cards;

public sealed class CardTests
{
    public class OnCloseButtonClick
    {
        [Fact]
        public async Task Invokes_event()
        {
            await using var ctx = new BunitContext();
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

            await component.Find(".close").ClickAsync();

            invoked.Should().BeTrue();
        }

        [Fact]
        public async Task Invokes_event_when_card_has_image()
        {
            await using var ctx = new BunitContext();
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

            await component.Find(".close").ClickAsync();

            invoked.Should().BeTrue();
        }
    }

    public class OnLinkClick
    {
        [Fact]
        public async Task Invokes_event()
        {
            await using var ctx = new BunitContext();
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

            var linkButton = component.FindComponent<Client.Components.LinkButton.LinkButton>();
            await linkButton.InvokeAsync(() => linkButton.Instance.OnClick.InvokeAsync(new MouseEventArgs()));

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
