using Sdk.Messaging;

namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents an error that occurred during an artifact query, along with the source that produced it.
/// </summary>
public interface IArtifactQueryError
{
    /// <summary>
    /// Gets the identifier of the source where the error occurred.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Gets structured information describing the error that occurred.
    /// </summary>
    ErrorInfo Error { get; }
}
