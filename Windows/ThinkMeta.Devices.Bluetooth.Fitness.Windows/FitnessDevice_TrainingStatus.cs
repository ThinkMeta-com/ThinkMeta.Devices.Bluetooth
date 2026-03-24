using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new training status data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, TrainingStatusData>? TrainingStatusChanged;

    private async void OnTrainingStatusChangedAsync(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        if (data.Length < 2)
            return;

        var flags = BitConverter.ToUInt16(data, 0);

        var trainingStatusData = new TrainingStatusData {
            Status = (TrainingStatus)data[1]
        };

        // Bit 0: Training Status String present
        if ((flags & 0x0001) != 0) {
            trainingStatusData.Text = Encoding.UTF8.GetString(data, 2, data.Length - 2);

            // Bit 1: Extended Training String present
            if ((flags & 0x0002) != 0) {
                var result = await sender.ReadValueAsync(BluetoothCacheMode.Uncached);
                if (result.Status == GattCommunicationStatus.Success) {
                    var extendedData = GetBytes(result.Value);
                    trainingStatusData.Text = Encoding.UTF8.GetString(extendedData, 1, extendedData.Length - 1);
                }
                else {
                    trainingStatusData.Text = string.Empty;
                }
            }
        }

        TrainingStatusChanged?.Invoke(this, trainingStatusData);
    }
}

