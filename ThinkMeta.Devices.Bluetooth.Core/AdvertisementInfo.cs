namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Stores information and RSSI statistics for a Bluetooth device discovered via advertisement.
/// </summary>
public class AdvertisementInfo
{
    // Exponential Weighted Moving Average (EWMA) smoothing
    private const double EwmaAlpha = 0.3; // Smoothing factor (0 < alpha <= 1)

    private sealed class RssiSample
    {
        public int Rssi { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    private List<RssiSample> RssiSamples { get; } = [];

    /// <summary>
    /// Gets the Bluetooth address of the device.
    /// </summary>
    public ulong BluetoothAddress { get; init; }

    /// <summary>
    /// Gets the display name of the device.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the device was last seen.
    /// </summary>
    public DateTimeOffset LastSeen { get; set; }

    /// <summary>
    /// Gets the exponentially weighted moving average (EWMA) of RSSI values.
    /// </summary>
    public double SmoothedRssi { get; private set; } = double.MinValue;

    /// <summary>
    /// Gets the median RSSI value from recent samples.
    /// </summary>
    public double MedianRssi => RssiSamples.Count > 0 ? GetMedianRssi() : double.MinValue;

    internal void AddRssiSample(int rssi, DateTimeOffset timestamp)
    {
        RssiSamples.Add(new RssiSample { Rssi = rssi, Timestamp = timestamp });
        var cutoff = timestamp.AddSeconds(-2);
        _ = RssiSamples.RemoveAll(s => s.Timestamp < cutoff);
        LastSeen = timestamp;

        // Update EWMA
        SmoothedRssi = SmoothedRssi == double.MinValue ? rssi : EwmaAlpha * rssi + (1 - EwmaAlpha) * SmoothedRssi;
    }

    private double GetMedianRssi()
    {
        if (RssiSamples.Count == 0)
            return double.MinValue;

        var sorted = new List<int>(RssiSamples.Count);
        foreach (var s in RssiSamples)
            sorted.Add(s.Rssi);

        sorted.Sort();
        var n = sorted.Count;

        if (n % 2 == 1)
            return sorted[n / 2];

        return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
}
