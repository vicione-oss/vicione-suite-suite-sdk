using System.Runtime.CompilerServices;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Default implementation of state for a notification element.
/// </summary>
public class NotificationElementState : INotificationElementState
{
    private readonly Lock _concurrentLock = new();
    private readonly HashSet<string> _changedProperties = [];
    private int _updateLock;
    private bool _isActive;
    private bool _visible = Constants.NotificationElementVisibleDefault;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value != _isActive)
            {
                if (value && !_visible)
                    Visible = true;

                _isActive = value;

                OnPropertyChanged();
            }
        }
    }

    public bool Visible
    {
        get => _visible;
        set
        {
            if (value != _visible)
            {
                _visible = value;

                if (!value && _isActive)
                    IsActive = false;

                OnPropertyChanged();
            }
        }
    }

    public event Action<NotificationElementStateChangedEventArgs>? Changed;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            return;

        if (_updateLock == 0)
            Changed?.Invoke(new NotificationElementStateChangedEventArgs(this, [propertyName]));
        else
            _changedProperties.Add(propertyName);
    }

    public void BeginUpdate()
    {
        lock (_concurrentLock)
        {
            _updateLock++;
        }
    }

    public void EndUpdate()
    {
        lock (_concurrentLock)
        {
            _updateLock--;

            if (_updateLock <= 0)
            {
                _updateLock = 0;

                if (_changedProperties.Count == 0)
                    return;

                Changed?.Invoke(new NotificationElementStateChangedEventArgs(this, _changedProperties));

                _changedProperties.Clear();
            }
        }
    }
}
