using System.Runtime.CompilerServices;
using ViciOne.SourceGenerators.NotifyChanged;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Provides a default implementation of <see cref="INotificationElementState"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[NotifyChanged]
public partial class NotificationElementState : INotificationElementState
{
    private readonly Lock _concurrentLock = new();
    private readonly HashSet<string> _changedProperties = [];
    private int _updateLock;

    /// <inheritdoc/>
    public partial bool IsActive { get; set; }

    /// <inheritdoc/>
    public partial bool Visible { get; set; } = Constants.NotificationElementVisibleDefault;

    /// <inheritdoc/>
    public event Action<NotificationElementStateChangedEventArgs>? Changed;

    partial void OnIsActiveChanging(bool value)
    {
        if (value && !Visible)
            Visible = true;
    }

    partial void OnVisibleChanged(bool value)
    {
        if (!value && IsActive)
            IsActive = false;
    }

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
