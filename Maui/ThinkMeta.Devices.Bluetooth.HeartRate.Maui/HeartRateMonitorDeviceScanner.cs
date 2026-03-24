using ThinkMeta.Devices.Bluetooth.Core.Maui;

namespace ThinkMeta.Devices.Bluetooth.HeartRate.Maui;

/// <summary>
/// Scans for Bluetooth heart rate monitor devices.
/// </summary>
public class HeartRateMonitorDeviceScanner : DeviceScanner
{
    /// <summary>
    /// Starts scanning for Bluetooth heart rate monitor devices.
    /// </summary>
    public Task StartScanningAsync() => StartScanningAsync(HeartRateMonitorDeviceGuids.HeartRateServiceUuid);
}

