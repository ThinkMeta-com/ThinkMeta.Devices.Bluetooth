using ThinkMeta.Devices.Bluetooth.Core.Windows;

namespace ThinkMeta.Devices.Bluetooth.HeartRate.Windows;

/// <summary>
/// Scans for Bluetooth heart rate monitor devices.
/// </summary>
public class HeartRateMonitorDeviceScanner : DeviceScanner
{
    /// <summary>
    /// Starts scanning for Bluetooth heart rate monitor devices.
    /// </summary>
    public void StartScanning() => StartScanning(HeartRateMonitorDeviceGuids.HeartRateServiceUuid);
}

