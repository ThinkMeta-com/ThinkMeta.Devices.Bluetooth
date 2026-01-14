namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Provides UUIDs for common Bluetooth Device Information services and characteristics.
/// </summary>
public static class GenericBluetoothGuids
{
    /// <summary>
    /// The UUID of the Device Information service.
    /// </summary>
    public static readonly Guid DeviceInformationServiceUuid = Guid.Parse("0000180A-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Manufacturer Name characteristic.
    /// </summary>
    public static readonly Guid ManufacturerNameUuid = Guid.Parse("00002A29-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the Model Number characteristic.
    /// </summary>
    public static readonly Guid ModelNumberUuid = Guid.Parse("00002A24-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the Serial Number characteristic.
    /// </summary>
    public static readonly Guid SerialNumberUuid = Guid.Parse("00002A25-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the Hardware Revision characteristic.
    /// </summary>
    public static readonly Guid HardwareRevisionUuid = Guid.Parse("00002A27-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the Firmware Revision characteristic.
    /// </summary>
    public static readonly Guid FirmwareRevisionUuid = Guid.Parse("00002A26-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the Software Revision characteristic.
    /// </summary>
    public static readonly Guid SoftwareRevisionUuid = Guid.Parse("00002A28-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the System ID characteristic.
    /// </summary>
    public static readonly Guid SystemIdUuid = Guid.Parse("00002A23-0000-1000-8000-00805f9b34fb");
    /// <summary>
    /// The UUID of the PnP ID characteristic.
    /// </summary>
    public static readonly Guid PnPIdUuid = Guid.Parse("00002A50-0000-1000-8000-00805f9b34fb");
}
