using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Extension methods for BluetoothLEDevice to retrieve device information.
/// </summary>
public static class BluetoothLEDeviceExtensions
{
    /// <summary>
    /// Asynchronously retrieves device information characteristics from a Bluetooth LE device.
    /// </summary>
    /// <param name="device">The Bluetooth LE device.</param>
    /// <returns>A <see cref="DeviceInformation"/> instance with available information.</returns>
    public static async Task<DeviceInformation> GetDeviceInformationAsync(this BluetoothLEDevice device)
    {
        var info = new DeviceInformation();
        var servicesResult = await device.GetGattServicesForUuidAsync(GenericBluetoothGuids.DeviceInformationServiceUuid);
        if (servicesResult.Status != GattCommunicationStatus.Success || servicesResult.Services.Count == 0)
            return info;

        var service = servicesResult.Services[0];

        async Task<string?> ReadCharacteristicAsync(Guid characteristicUuid)
        {
            try {
                var result = await service.GetCharacteristicsForUuidAsync(characteristicUuid);
                if (result.Status == GattCommunicationStatus.Success && result.Characteristics.Count > 0) {
                    var readResult = await result.Characteristics[0].ReadValueAsync();
                    if (readResult.Status == GattCommunicationStatus.Success) {
                        var reader = DataReader.FromBuffer(readResult.Value);
                        return reader.ReadString(readResult.Value.Length);
                    }
                }
            }
            catch { /* ignore */ }

            return null;
        }

        info.ManufacturerName = await ReadCharacteristicAsync(GenericBluetoothGuids.ManufacturerNameUuid);
        info.ModelNumber = await ReadCharacteristicAsync(GenericBluetoothGuids.ModelNumberUuid);
        info.SerialNumber = await ReadCharacteristicAsync(GenericBluetoothGuids.SerialNumberUuid);
        info.HardwareRevision = await ReadCharacteristicAsync(GenericBluetoothGuids.HardwareRevisionUuid);
        info.FirmwareRevision = await ReadCharacteristicAsync(GenericBluetoothGuids.FirmwareRevisionUuid);
        info.SoftwareRevision = await ReadCharacteristicAsync(GenericBluetoothGuids.SoftwareRevisionUuid);

        return info;
    }
}