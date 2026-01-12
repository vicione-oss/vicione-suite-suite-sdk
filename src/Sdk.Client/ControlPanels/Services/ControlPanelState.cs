using System.Runtime.CompilerServices;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Default implementation of <see cref="IControlPanelState"/>
/// </summary>
public class ControlPanelState : IControlPanelState
{
    private readonly Lock _concurrentLock = new();
    private int _loadingCounter;
    private bool _isLoading;
    private int? _activeControlPanelPageIndex;

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (value != _isLoading)
            {
                _isLoading = value;

                OnPropertyChanged();
            }
        }
    }

    public int? ActivePageIndex
    {
        get => _activeControlPanelPageIndex;
        set
        {
            if (value != _activeControlPanelPageIndex)
            {
                _activeControlPanelPageIndex = value;

                OnPropertyChanged();
            }
        }
    }

    public event Action<ControlPanelStateChangedEventArgs>? Changed;

    public void BeginLoading()
    {
        lock (_concurrentLock)
        {
            _loadingCounter++;

            if (_loadingCounter == 1)
                IsLoading = true;
        }
    }

    public void EndLoading()
    {
        lock (_concurrentLock)
        {
            if (_loadingCounter > 0)
                _loadingCounter--;

            if (_loadingCounter == 0)
                IsLoading = false;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            return;

        Changed?.Invoke(new ControlPanelStateChangedEventArgs(this, [propertyName]));
    }
}
