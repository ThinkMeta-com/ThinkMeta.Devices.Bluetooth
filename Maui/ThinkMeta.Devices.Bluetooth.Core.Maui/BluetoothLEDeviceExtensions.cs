using System.Text;
using Plugin.BLE.Abstractions.Contracts;

namespace ThinkMeta.Devices.Bluetooth.Core.Maui;

/// <summary>
/// Extension methods for Plugin.BLE IDevice to retrieve device information.
/// </summary>
public static class BluetoothLEDeviceExtensions
{
    /// <summary>
    /// Asynchronously retrieves device information characteristics from a Bluetooth LE device.
    /// </summary>
    /// <param name="device">The Plugin.BLE device.</param>
    /// <returns>A <see cref="DeviceInformation"/> instance with available information.</returns>
    public static async Task<DeviceInformation> GetDeviceInformationAsync(this IDevice device)
    {
        var info = new DeviceInformation();
        try {
            var service = await device.GetServiceAsync(GenericBluetoothGuids.DeviceInformationServiceUuid);
            if (service is null)
                return info;

            info.ManufacturerName = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.ManufacturerNameUuid);
            info.ModelNumber = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.ModelNumberUuid);
            info.SerialNumber = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.SerialNumberUuid);
            info.HardwareRevision = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.HardwareRevisionUuid);
            info.FirmwareRevision = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.FirmwareRevisionUuid);
            info.SoftwareRevision = await ReadStringCharacteristicAsync(service, GenericBluetoothGuids.SoftwareRevisionUuid);
        }
        catch { /* ignore */ }

        return info;
    }

    private static async Task<string?> ReadStringCharacteristicAsync(IService service, Guid characteristicUuid)
    {
        try {
            var characteristic = await service.GetCharacteristicAsync(characteristicUuid);
            if (characteristic is null)
                return null;

            var (data, _) = await characteristic.ReadAsync();
            return data is { Length: > 0 } ? Encoding.UTF8.GetString(data) : null;
        }
        catch { /* ignore */ }

        return null;
    }
}

