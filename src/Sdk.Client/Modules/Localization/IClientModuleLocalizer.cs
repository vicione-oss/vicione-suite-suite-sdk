namespace Sdk.Client.Modules.Localization;

/// <summary>
/// Defines methods to retrieve localized text for a client module.
/// </summary>
public interface IClientModuleLocalizer<out TClientModule>
    where TClientModule : IClientModule
{
    /// <summary>
    /// Gets the localized title of the module.
    /// </summary>
    string GetTitle();

    /// <summary>
    /// Gets the optional localized description of the module.
    /// </summary>
    string? GetDescription() => null;
}
