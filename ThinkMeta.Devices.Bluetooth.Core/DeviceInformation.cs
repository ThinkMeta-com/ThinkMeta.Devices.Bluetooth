namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Stores device information characteristics for a Bluetooth LE device.
/// </summary>
public class DeviceInformation
{
    /// <summary>
    /// Gets or sets the manufacturer name.
    /// </summary>
    public string? ManufacturerName { get; set; }
    /// <summary>
    /// Gets or sets the model number.
    /// </summary>
    public string? ModelNumber { get; set; }
    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    public string? SerialNumber { get; set; }
    /// <summary>
    /// Gets or sets the hardware revision.
    /// </summary>
    public string? HardwareRevision { get; set; }
    /// <summary>
    /// Gets or sets the firmware revision.
    /// </summary>
    public string? FirmwareRevision { get; set; }
    /// <summary>
    /// Gets or sets the software revision.
    /// </summary>
    public string? SoftwareRevision { get; set; }
}
