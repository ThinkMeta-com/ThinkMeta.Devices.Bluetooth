using System.Text;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new training status data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, TrainingStatusData>? TrainingStatusChanged;

    private async void OnTrainingStatusReceivedAsync(byte[] data, Plugin.BLE.Abstractions.Contracts.ICharacteristic characteristic)
    {
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
                try {
                    var (extendedData, _) = await characteristic.ReadAsync();
                    if (extendedData is { Length: > 1 })
                        trainingStatusData.Text = Encoding.UTF8.GetString(extendedData, 1, extendedData.Length - 1);
                    else
                        trainingStatusData.Text = string.Empty;
                }
                catch {
                    trainingStatusData.Text = string.Empty;
                }
            }
        }

        TrainingStatusChanged?.Invoke(this, trainingStatusData);
    }
}

