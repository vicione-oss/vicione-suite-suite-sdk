using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Implements the state for a navigation tile component.
/// </summary>
public class NavTileState
{
    private bool _enabled;
    private int _loadingCounter;
    private string? _linkTarget;

    /// <summary>
    /// Gets or initializes the horizontal span of the tile, which determines its width.
    /// </summary>
    public NavTileSpan HorizontalSpan { get; init; }

    /// <summary>
    /// Gets or sets whether the tile is enabled and interactive.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value != _enabled)
            {
                _enabled = value;

                Changed?.Invoke();
            }
        }
    }

    /// <summary>
    /// Gets whether the tile is currently in a loading state.
    /// </summary>
    public bool IsLoading { get; private set; }

    /// <summary>
    /// Gets or sets the URL that the tile navigates to when clicked.
    /// </summary>
    public string? LinkTarget
    {
        get => _linkTarget;
        set
        {
            if (value != _linkTarget)
            {
                _linkTarget = value;

                Changed?.Invoke();
            }
        }
    }

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
}
