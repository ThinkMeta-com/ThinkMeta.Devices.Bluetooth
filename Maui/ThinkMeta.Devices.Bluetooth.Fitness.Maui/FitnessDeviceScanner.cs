using Plugin.BLE.Abstractions;
using ThinkMeta.Devices.Bluetooth.Core.Maui;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

/// <summary>
/// Scans for FTMS (Fitness Machine Service) Bluetooth devices.
/// </summary>
public class FitnessDeviceScanner : DeviceScanner
{
    /// <summary>
    /// Starts scanning for FTMS Bluetooth devices.
    /// </summary>
    public Task StartScanningAsync() => StartScanningAsync(FitnessDeviceGuids.FtmsServiceUuid);

    /// <inheritdoc />
    protected override AdvertisementInfo CreateAdvertisementInfo(Plugin.BLE.Abstractions.Contracts.IDevice device)
    {
        var info = new FitnessMachineAdvertisementInfo {
            DeviceId = device.Id,
            Name = device.Name ?? string.Empty,
            LastSeen = DateTimeOffset.UtcNow,
            Device = device,
            MachineTypes = GetDeviceTypesFromAdvertisement(device)
        };

        return info;
    }

    private static FitnessMachineTypes GetDeviceTypesFromAdvertisement(Plugin.BLE.Abstractions.Contracts.IDevice device)
    {
        foreach (var record in device.AdvertisementRecords) {
            if (record.Type != AdvertisementRecordType.ServiceData)
                continue;

            var data = record.Data;
            if (data.Length < 5)
                continue;

            var uuid = (ushort)(data[1] << 8 | data[0]);
            if (uuid != 0x1826) // FTMS UUID
                continue;

            // check flag "Fitness Machine Available"
            var flags = data[2];
            if ((flags & 0b00000001) == 0)
                return FitnessMachineTypes.None;

            // some devices send the type in big-endian format which is wrong according to the spec
            return data[3] == 0 ? (FitnessMachineTypes)data[4] : (FitnessMachineTypes)(data[4] << 8 | data[3]);
        }

        return FitnessMachineTypes.None;
    }
}

