using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents the current training status as defined by the FTMS specification.
/// </summary>
public enum TrainingStatus
{
    /// <summary>Other or unknown status.</summary>
    Other = 0,
    /// <summary>Idle.</summary>
    Idle = 1,
    /// <summary>Warming up.</summary>
    WarmingUp = 2,
    /// <summary>Low intensity interval.</summary>
    LowIntensityInterval = 3,
    /// <summary>High intensity interval.</summary>
    HighIntensityInterval = 4,
    /// <summary>Recovery interval.</summary>
    RecoveryInterval = 5,
    /// <summary>Isometric.</summary>
    Isometric = 6,
    /// <summary>Heart rate control.</summary>
    HeartRateControl = 7,
    /// <summary>Fitness test.</summary>
    FitnessTest = 8,
    /// <summary>Speed outside of control region (low).</summary>
    SpeedOutsideOfControlRegionLow = 9,
    /// <summary>Speed outside of control region (high).</summary>
    SpeedOutsideOfControlRegionHigh = 10,
    /// <summary>Cool down.</summary>
    CoolDown = 11,
    /// <summary>Watt control.</summary>
    WattControl = 12,
    /// <summary>Manual mode.</summary>
    ManualMode = 13,
    /// <summary>Pre-workout.</summary>
    PreWorkout = 14,
    /// <summary>Post-workout.</summary>
    PostWorkout = 15
}

/// <summary>
/// Represents training status data from the FTMS characteristic.
/// </summary>
public class TrainingStatusData
{
    /// <summary>
    /// Gets or sets the current training status.
    /// </summary>
    public TrainingStatus Status { get; set; }
    /// <summary>
    /// Gets or sets the training status text, if present.
    /// </summary>
    public string? Text { get; set; }
}

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
