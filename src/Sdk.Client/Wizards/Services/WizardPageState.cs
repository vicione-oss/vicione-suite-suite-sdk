using System.Runtime.CompilerServices;
using Sdk.Client.Models;
using Sdk.Client.Wizards.Models;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Provides a default implementation of <see cref="IWizardPageState"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class WizardPageState : IWizardPageState
{
    private readonly Lock _concurrentLock = new();
    private int _operationCounter;
    private IWizardOperation? _currentOperation;

    /// <inheritdoc/>
    public IWizardOperation? CurrentOperation
    {
        get => _currentOperation;
        private set
        {
            if (value != _currentOperation)
            {
                _currentOperation = value;

                OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc/>
    public event Action<PropertiesChangedEventArgs>? Changed;

    /// <inheritdoc/>
    public void BeginOperation(IWizardOperation operation)
    {
        lock (_concurrentLock)
        {
            _operationCounter++;

            if (_operationCounter == 1)
                CurrentOperation = operation;
        }
    }

    /// <inheritdoc/>
    public void EndOperation()
    {
        lock (_concurrentLock)
        {
            if (_operationCounter > 0)
                _operationCounter--;

            if (_operationCounter == 0)
                CurrentOperation = null;
        }
    }

    /// <summary>
    /// Raises the <see cref="Changed"/> event for a specific property.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            return;

        Changed?.Invoke(new PropertiesChangedEventArgs(this, new HashSet<string> { propertyName }));
    }
}
