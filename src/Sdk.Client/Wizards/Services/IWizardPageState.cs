using Sdk.Client.Interfaces;
using Sdk.Client.Wizards.Models;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// The state of a wizard page, kept outside the component's render cycle.
/// </summary>
public interface IWizardPageState : IHasChangeableProperties
{
    /// <summary>
    /// Gets the operation passed to the first open <see cref="BeginOperation"/>; <see langword="null"/> when none is running.
    /// </summary>
    IWizardOperation? CurrentOperation { get; }

    /// <summary>
    /// Starts an operation cycle. Cycles nest: only the first open cycle sets <see cref="CurrentOperation"/> to
    /// <paramref name="operation"/> and raises <see cref="IHasChangeableProperties.Changed"/>; every call needs a matching
    /// <see cref="EndOperation"/>.
    /// </summary>
    void BeginOperation(IWizardOperation operation);

    /// <summary>
    /// Ends an operation cycle; closing the last one clears <see cref="CurrentOperation"/> and raises
    /// <see cref="IHasChangeableProperties.Changed"/>. A call without an open cycle is ignored.
    /// </summary>
    void EndOperation();
}
