using System.Runtime.InteropServices.WindowsRuntime;
using ThinkMeta.Devices.Bluetooth.Core;
using Windows.Devices.Bluetooth.Advertisement;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Scans for FTMS (Fitness Machine Service) Bluetooth devices.
/// </summary>
public class FitnessDeviceScanner : DeviceScanner
{
    /// <summary>
    /// Starts scanning for FTMS Bluetooth devices.
    /// </summary>
    public void StartScanning() => StartScanning(FitnessDeviceGuids.FtmsServiceUuid);

    /// <inheritdoc />
    protected override AdvertisementInfo CreateAdvertisementInfo(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var info = new FitnessMachineAdvertisementInfo {
            BluetoothAddress = args.BluetoothAddress,
            Name = args.Advertisement.LocalName ?? string.Empty,
            LastSeen = DateTimeOffset.UtcNow,
            MachineTypes = GetDeviceTypesFromAdvertisement(args)
        };

        return info;
    }

    private static FitnessMachineTypes GetDeviceTypesFromAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        foreach (var section in args.Advertisement.DataSections) {
            if (section.DataType != 0x16)
                continue; // Service Data - 16-bit UUID

            var data = section.Data.ToArray();

            if (data.Length < 5)
                continue;

            var uuid = (ushort)(data[1] << 8 | data[0]);
            if (uuid != 0x1826) // FTMS UUID
                continue;

            // check flag "Fitness Machine Available"
            var flags = data[2];
            if ((flags & 0b00000001) == 0)
                return FitnessMachineTypes.None;

            return data[3] == 0 ? (FitnessMachineTypes)data[4] : (FitnessMachineTypes)(data[4] << 8 | data[3]);
        }

        return FitnessMachineTypes.None;
    }
}
