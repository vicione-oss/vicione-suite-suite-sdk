namespace Sdk.Journal;

/// <summary>
/// Defines a contract for a service that monitors configurable journal parts.
/// </summary>
public interface IJournalMonitoring
{
    /// <summary>
    /// Gets a read-only dictionary of the currently configured journal filter entries.
    /// </summary>
    IReadOnlyDictionary<string, JournalFilterEntry> FilterEntries { get; }

    /// <summary>
    /// Adds a new filter entry to be monitored.
    /// </summary>
    void AddFilterEntry(string name, string displayName, string filter, int index);
}
