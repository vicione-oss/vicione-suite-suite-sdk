namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Defines a factory for creating wizard page registries.
/// </summary>
/// <remarks>
/// This interface is intended for internal framework use only and should not be used directly by consumer applications.
/// </remarks>
public interface IWizardPageRegistryFactory
{
    /// <summary>
    /// Creates a new wizard page registry associated with <typeparamref name="TContext"/>.
    /// </summary>
    IWizardPageRegistry<TContext> CreateWizardPageRegistry<TContext>();
}
