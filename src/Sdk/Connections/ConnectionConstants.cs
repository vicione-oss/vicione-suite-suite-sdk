using System.Diagnostics.CodeAnalysis;
using Sdk.Connections.Contracts;

namespace Sdk.Connections;

/// <summary>
/// Provides constants for connection management including predefined tags and metadata keys.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public static class ConnectionConstants
{
    /// <summary>
    /// Provides predefined tags for connection categorization and management.
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// Gets the system default tag that is automatically assigned to new connections.
        /// </summary>
        /// <value>
        /// A protected <see cref="Tag"/> with the text "SystemDefault" and a fixed GUID identifier.
        /// This tag cannot be modified or deleted due to its <see cref="Tag.Protected"/> property.
        /// </value>
        public static Tag SystemDefault { get; }
            = new("SystemDefault", new Guid("58CF1C71-B99F-4F7E-A767-B24EBC1E599E")) { Protected = true };
    }

    /// <summary>
    /// Provides standardized metadata key names for connection properties.
    /// </summary>
    public static class MetaDataKeys
    {
        /// <summary>
        /// The metadata key for storing the instance identifier of a connection.
        /// </summary>
        public const string InstanceId = "InstanceId";

        /// <summary>
        /// The metadata key for storing the MQTT client protocol version or configuration.
        /// </summary>
        public const string MqttClientProtocol = "MqttClientProtocol";
    }
}
