namespace ThinkMeta.Devices.Bluetooth.Core.Windows;

/// <summary>
/// Stores information and RSSI statistics for a Bluetooth device discovered via advertisement.
/// </summary>
public class AdvertisementInfo : AdvertisementInfoBase
{
    /// <summary>
    /// Gets the Bluetooth address of the device.
    /// </summary>
    public ulong BluetoothAddress { get; init; }
}
