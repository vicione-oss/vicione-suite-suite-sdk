namespace Sdk.Connections.Contracts;

/// <summary>
/// Specifies the quality of service level used for an MQTT connection.
/// </summary>
public enum MqttQualityOfServiceLevel
{
    /// <summary>
    /// QoS 0: The message is delivered at most once, and delivery is not guaranteed. This is the fastest mode but may result in lost messages.
    /// </summary>
    AtMostOnce,

    /// <summary>
    /// QoS 1: The message is delivered at least once, ensuring that it reaches the receiver but may result in duplicate messages.
    /// </summary>
    AtLeastOnce,

    /// <summary>
    /// QoS 2: The message is delivered exactly once by using a four-step handshake. This is the safest and slowest mode, ensuring that messages are neither lost nor duplicated.
    /// </summary>
    ExactlyOnce
}
