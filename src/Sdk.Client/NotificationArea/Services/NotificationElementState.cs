using System.Runtime.CompilerServices;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Provides a default implementation of <see cref="INotificationElementState"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class NotificationElementState : INotificationElementState
{
    private readonly Lock _concurrentLock = new();
    private readonly HashSet<string> _changedProperties = [];
    private int _updateLock;
    private bool _isActive;
    private bool _visible = Constants.NotificationElementVisibleDefault;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public event Action<NotificationElementStateChangedEventArgs>? Changed;

    /// <summary>
    /// Raises the <see cref="Changed"/> event for a specific property.
    /// If an update lock entered via <see cref="BeginUpdate"/> is active, the property name is buffered
    /// and the event is raised later when the lock is released with <see cref="EndUpdate"/>.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            return;

        // Checked and buffered under one lock, so an EndUpdate running in between cannot strand the name in the buffer.
        lock (_concurrentLock)
        {
            if (_updateLock > 0)
            {
                _changedProperties.Add(propertyName);

                return;
            }
        }

        Changed?.Invoke(new NotificationElementStateChangedEventArgs(this, [propertyName]));
    }

    /// <inheritdoc/>
    public void BeginUpdate()
    {
        lock (_concurrentLock)
        {
            _updateLock++;
        }
    }

    /// <inheritdoc/>
    public void EndUpdate()
    {
        HashSet<string> changedProperties;

        lock (_concurrentLock)
        {
            _updateLock--;

            if (_updateLock > 0)
                return;

            _updateLock = 0;

            if (_changedProperties.Count == 0)
                return;

            changedProperties = [.. _changedProperties];

            _changedProperties.Clear();
        }

        Changed?.Invoke(new NotificationElementStateChangedEventArgs(this, changedProperties));
    }
}
