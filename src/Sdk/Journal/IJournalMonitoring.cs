namespace Sdk.Journal;

/// <summary>
/// This is an interface for a service that monitors configurable journal parts. The implementation should provide
/// thread-safe operations. The monitoring creates a published statistic for each entry.
/// </summary>
public interface IJournalMonitoring
{
    IReadOnlyDictionary<string, JournalFilterEntry> FilterEntries { get; }
    void AddFilterEntry(string name, string displayName, string filter, int index);
}
