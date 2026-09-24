namespace Sdk.Client.Wizards.Services;

/// <summary>
/// Describes how a wizard page is presented.
/// </summary>
public interface IWizardPageDescriptor
{
    /// <summary>
    /// Gets the page's title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the page's position in the wizard; lower positions come first in navigation. Pages without a position follow
    /// in registration order. Defaults to <see langword="null"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    int? Position => null;
}
