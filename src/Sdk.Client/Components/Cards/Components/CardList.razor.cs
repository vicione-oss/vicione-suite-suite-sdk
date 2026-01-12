using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Cards.Components;

/// <summary>
/// A component that provides a layout container for a list of cards.
/// </summary>
public sealed partial class CardList
{
    /// <summary>
    /// Gets or sets the child content of the component, which should consist of one or more <see cref="Card"/> components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
