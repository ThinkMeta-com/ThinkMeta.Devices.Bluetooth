namespace ThinkMeta.Devices.Bluetooth.Core.Maui;

/// <summary>
/// Stores information and RSSI statistics for a Bluetooth device discovered via advertisement.
/// </summary>
public class AdvertisementInfo : AdvertisementInfoBase
{
    /// <summary>
    /// Gets the device identifier.
    /// </summary>
    public Guid DeviceId { get; init; }

    /// <summary>
    /// Gets the Plugin.BLE device reference used for connecting.
    /// </summary>
    public Plugin.BLE.Abstractions.Contracts.IDevice Device { get; init; } = default!;
}

