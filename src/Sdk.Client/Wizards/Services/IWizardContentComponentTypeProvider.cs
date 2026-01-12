namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Defines a provider for retrieving the type of the component that renders the wizard's content.
/// </summary>
/// <remarks>
/// This interface is intended for internal framework use only and should not be used directly by consumer applications.
/// </remarks>
public interface IWizardContentComponentTypeProvider
{
    /// <summary>
    /// Gets the component type that implements the content for a wizard associated with <typeparamref name="TContext"/>.
    /// </summary>
    Type GetWizardContentComponentType<TContext>();
}
