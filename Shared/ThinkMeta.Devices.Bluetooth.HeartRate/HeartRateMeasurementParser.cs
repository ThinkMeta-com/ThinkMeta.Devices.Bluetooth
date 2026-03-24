namespace ThinkMeta.Devices.Bluetooth.HeartRate;

/// <summary>
/// Parses heart rate measurement data from a GATT characteristic byte array.
/// </summary>
public static class HeartRateMeasurementParser
{
    /// <summary>
    /// Parses the heart rate value from a Heart Rate Measurement characteristic byte array.
    /// </summary>
    /// <param name="data">The raw byte data from the Heart Rate Measurement characteristic.</param>
    /// <returns>The heart rate in BPM, or <see langword="null"/> if the data is too short.</returns>
    public static int? Parse(byte[] data)
    {
        if (data is not { Length: >= 2 })
            return null;

        var flags = data[0];

        // Bit 0: Heart Rate Value Format (0 = uint8, 1 = uint16)
        return (flags & 0x01) == 0 ? data[1] : data[1] | (data[2] << 8);
    }
}
