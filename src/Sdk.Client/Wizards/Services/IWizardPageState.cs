using System.Threading.Channels;
using Sdk.Client.Interfaces;
using Sdk.Client.Wizards.Models;

namespace Sdk.Client.Wizards.Services;

/// <summary>
/// State for a wizard page
/// </summary>
public interface IWizardPageState : IHasChangeableProperties
{
    /// <summary>
    /// Returns the current operation set in a call to <see cref="BeginOperation"/>, otherwise returns <see langword="null"/>.
    /// </summary>
    IWizardOperation? CurrentOperation { get; }

    /// <summary>
    /// Call this method to begin a operation cycle.
    ///
    /// <para>
    /// This increments the internal operation counter, which initially is zero. The counter counts the number of times
    /// <see cref="BeginOperation"/> was called without a corresponding call to <see cref="EndOperation"/>.
    /// </para>
    ///
    /// <para>
    /// If the internal operation counter was zero on method entry then <see cref="CurrentOperation" /> is set and
    /// the <see cref="IHasChangeableProperties.Changed"/> event is raised.
    /// </para>
    ///
    /// Make sure to have a corresponding call to <see cref="EndOperation"/> to end the operation cycle.
    /// </summary>
    /// <param name="operation">Describes the wizard operation</param>
    void BeginOperation(IWizardOperation operation);

    /// <summary>
    /// Call this method to end a operation cycle. This decrements the internal operation counter.
    ///
    /// <para>
    /// If the internal operation counter reaches zero then <see cref="CurrentOperation" /> is set to <see langword="null"/> and
    /// the <see cref="Channel"/> event is raised.
    /// </para>
    /// </summary>
    void EndOperation();
}
