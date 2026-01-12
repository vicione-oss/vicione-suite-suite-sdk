using System.Runtime.CompilerServices;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Provides a default implementation of <see cref="IControlPanelState"/>.
/// </summary>
public class ControlPanelState : IControlPanelState
{
    private readonly Lock _concurrentLock = new();
    private readonly HashSet<string> _changedProperties = [];

    private int _loadingCounter;
    private bool _isLoading;
    private int? _activeControlPanelPageIndex;

    /// <inheritdoc/>
    public int UpdateLock { get; private set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public event Action<ControlPanelStateChangedEventArgs>? Changed;

    /// <inheritdoc/>
    public void BeginLoading()
    {
        lock (_concurrentLock)
        {
            _loadingCounter++;

            if (_loadingCounter == 1)
                IsLoading = true;
        }
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Raises the <see cref="Changed"/> event for a specific property.
    /// If an update lock is active, the property name is buffered and the event is raised later when the lock is released.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            return;

        if (UpdateLock == 0)
        {
            Changed?.Invoke(new ControlPanelStateChangedEventArgs(this, new HashSet<string> { propertyName }));
        }
        else
        {
            lock (_concurrentLock)
            {
                _changedProperties.Add(propertyName);
            }
        }
    }

    /// <inheritdoc/>
    public void BeginUpdate()
    {
        lock (_concurrentLock)
        {
            UpdateLock++;
        }
    }

    /// <inheritdoc/>
    public void EndUpdate()
    {
        HashSet<string> changedProperties;

        lock (_concurrentLock)
        {
            UpdateLock--;

            if (UpdateLock > 0)
                return;

            UpdateLock = 0;

            if (_changedProperties.Count == 0)
                return;

            changedProperties = [.. _changedProperties];

            _changedProperties.Clear();
        }

        Changed?.Invoke(new ControlPanelStateChangedEventArgs(this, changedProperties));
    }
}
