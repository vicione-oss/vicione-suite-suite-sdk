using Sdk.Client.NavTiles.Enums;

namespace Sdk.Client.NavTiles.Components;

public class NavTileState
{
    private bool _enabled;
    private int _loadingCounter;
    private string? _linkTarget;

    public NavTileSpan HorizontalSpan { get; init; }

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

    public bool IsLoading { get; private set; }

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

    public event Action? Changed;

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
