namespace ThinkMeta.Devices.Bluetooth.HeartRate;

/// <summary>
/// Provides UUIDs for the Heart Rate Service and its Measurement Characteristic.
/// </summary>
public static class HeartRateMonitorDeviceGuids
{
    /// <summary>
    /// The UUID of the Heart Rate Service.
    /// </summary>
    public static readonly Guid HeartRateServiceUuid = Guid.Parse("0000180D-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Heart Rate Measurement Characteristic.
    /// </summary>
    public static readonly Guid HeartRateMeasurementCharacteristicUuid = Guid.Parse("00002A37-0000-1000-8000-00805f9b34fb");
}
