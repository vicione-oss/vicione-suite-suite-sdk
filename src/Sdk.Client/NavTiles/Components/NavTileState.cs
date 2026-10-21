using Sdk.Client.NavTiles.Enums;
using ViciOne.SourceGenerators.NotifyChanged;

namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Implements the state for a navigation tile component.
/// </summary>
public partial class NavTileState
{
    private int _loadingCounter;

    /// <summary>
    /// Gets or initializes the horizontal span of the tile, which determines its width.
    /// </summary>
    public NavTileSpan HorizontalSpan { get; init; }

    /// <summary>
    /// Gets or sets whether the tile is enabled and interactive.
    /// </summary>
    [NotifyChanged]
    public partial bool Enabled { get; set; }

    /// <summary>
    /// Gets whether the tile is currently in a loading state.
    /// </summary>
    public bool IsLoading { get; private set; }

    /// <summary>
    /// Gets or sets the URL that the tile navigates to when clicked.
    /// </summary>
    [NotifyChanged]
    public partial string? LinkTarget { get; set; }

    /// <summary>
    /// Occurs when a property value in the state has changed.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Signals the beginning of a loading operation on the tile.
    /// This increments a counter, and if it's the first operation, sets <see cref="IsLoading"/> to true.
    /// </summary>
    public void BeginLoading()
    {
        _loadingCounter++;

        if (_loadingCounter == 1)
        {
            if (!IsLoading)
            {
                IsLoading = true;

                Changed?.Invoke();
            }
        }
    }

    /// <summary>
    /// Signals the end of a loading operation on the tile.
    /// This decrements a counter, and if it's the last operation, sets <see cref="IsLoading"/> to false.
    /// </summary>
    public void EndLoading()
    {
        if (_loadingCounter > 0)
            _loadingCounter--;

        if (_loadingCounter == 0)
        {
            IsLoading = false;

            Changed?.Invoke();
        }
    }

    /// <summary>
    /// Raises the <see cref="Changed"/> event; the setters generated for <see cref="NotifyChangedAttribute"/> call it.
    /// </summary>
    private void OnPropertyChanged()
        => Changed?.Invoke();
}
