namespace Sdk.Connections.Contracts;

/// <summary>
/// Specifies the quality of service level used for an MQTT connection.
/// </summary>
public enum MqttQualityOfServiceLevel
{
    /// <summary>
    /// QoS 0: delivered at most once, without a guarantee; the fastest mode, but messages can be lost.
    /// </summary>
    AtMostOnce,

    /// <summary>
    /// QoS 1: delivered at least once; nothing is lost, but duplicates are possible.
    /// </summary>
    AtLeastOnce,

    /// <summary>
    /// QoS 2: delivered exactly once through a four-step handshake; the safest and slowest mode.
    /// </summary>
    ExactlyOnce
}
