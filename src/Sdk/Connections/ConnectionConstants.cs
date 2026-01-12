using System.Diagnostics.CodeAnalysis;
using Sdk.Connections.Contracts;

namespace Sdk.Connections;

[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public static class ConnectionConstants
{
    public static class Tags
    {
        public static Tag SystemDefault { get; }
            = new("SystemDefault", new Guid("58CF1C71-B99F-4F7E-A767-B24EBC1E599E")) { Protected = true };
    }

    public static class MetaDataKeys
    {
        public const string InstanceId = "InstanceId";
        public const string MqttClientProtocol = "MqttClientProtocol";
    }
}
