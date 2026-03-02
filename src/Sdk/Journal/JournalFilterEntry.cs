namespace Sdk.Journal;

/// <summary>
/// Represents a configured filter for monitoring journal entries.
/// </summary>
[ExcludeFromCodeCoverage]
public record JournalFilterEntry(string DisplayName, string Filter, int Index);
