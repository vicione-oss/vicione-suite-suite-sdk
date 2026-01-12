namespace Sdk.Modules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "other override methods not necessary")]
public readonly struct ModuleKey
{
    public string ModuleId { get; init; }

    public ModuleType ModuleType { get; init; }

    public override string ToString() => $"{nameof(ModuleKey)} Id:{ModuleId} Type:{ModuleType}";
}
