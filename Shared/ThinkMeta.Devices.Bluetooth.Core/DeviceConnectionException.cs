namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Represents errors that occur during device connection.
/// </summary>
public class DeviceConnectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceConnectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DeviceConnectionException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceConnectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="inner">The inner exception reference.</param>
    public DeviceConnectionException(string message, Exception inner) : base(message, inner) { }
}
