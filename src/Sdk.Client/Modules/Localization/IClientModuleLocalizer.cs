namespace Sdk.Client.Modules.Localization;

/// <summary>
/// This interface provides methods to retrieve localized text for a <see cref="IClientModule">client module</see>.
/// </summary>
public interface IClientModuleLocalizer<out TClientModule>
    where TClientModule : IClientModule
{
    string GetTitle();
    string? GetDescription() => null;
}
