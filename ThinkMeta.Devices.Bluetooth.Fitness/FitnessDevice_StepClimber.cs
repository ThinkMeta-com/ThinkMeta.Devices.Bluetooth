using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents step climber measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class StepClimberData
{
    /// <summary>Floors climbed (count).</summary>
    public int? Floors { get; set; }
    /// <summary>Step count (count).</summary>
    public int? StepCount { get; set; }
    /// <summary>Steps per minute (steps/minute).</summary>
    public int? StepsPerMinute { get; set; }
    /// <summary>Average step rate (steps/minute).</summary>
    public int? AverageStepRate { get; set; }
    /// <summary>Positive elevation gain (meters).</summary>
    public int? PositiveElevationGain { get; set; }
    /// <summary>Total energy (kcal).</summary>
    public int? TotalEnergy { get; set; }
    /// <summary>Energy per hour (kcal/hour).</summary>
    public int? EnergyPerHour { get; set; }
    /// <summary>Energy per minute (kcal/minute).</summary>
    public int? EnergyPerMinute { get; set; }
    /// <summary>Heart rate (bpm).</summary>
    public int? HeartRate { get; set; }
    /// <summary>Metabolic equivalent (MET).</summary>
    public int? MetabolicEquivalent { get; set; }
    /// <summary>Elapsed time (seconds).</summary>
    public int? ElapsedTime { get; set; }
    /// <summary>Remaining time (seconds).</summary>
    public int? RemainingTime { get; set; }
}

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new step climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StepClimberData>? StepClimberDataChanged;

    private void OnStepClimberDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        if (data.Length < 2)
            return;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var stepClimberData = new StepClimberData();

        // Bit 0: More Data (0 = Floors present)
        if ((flags & 0x01) == 0 && offset + 4 <= data.Length) {
            stepClimberData.Floors = BitConverter.ToUInt16(data, offset);
            stepClimberData.StepCount = BitConverter.ToUInt16(data, offset + 2);
            offset += 4;
        }

        // Bit 1: Steps Per Minute present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) {
            stepClimberData.StepsPerMinute = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 2: Average Step Rate present
        if ((flags & 0x04) != 0 && offset + 2 <= data.Length) {
            stepClimberData.AverageStepRate = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 3: Positive Elevation Gain present
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) {
            stepClimberData.PositiveElevationGain = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 4: Energy Data present
        if ((flags & 0x10) != 0 && offset + 5 <= data.Length) {
            stepClimberData.TotalEnergy = BitConverter.ToUInt16(data, offset);
            stepClimberData.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2);
            stepClimberData.EnergyPerMinute = data[offset + 4];
            offset += 5;
        }

        // Bit 5: Heart Rate present
        if ((flags & 0x20) != 0 && offset + 1 <= data.Length) {
            stepClimberData.HeartRate = data[offset];
            offset += 1;
        }

        // Bit 6: Metabolic Equivalent present
        if ((flags & 0x40) != 0 && offset + 1 <= data.Length) {
            stepClimberData.MetabolicEquivalent = data[offset];
            offset += 1;
        }

        // Bit 7: Elapsed Time present
        if ((flags & 0x80) != 0 && offset + 2 <= data.Length) {
            stepClimberData.ElapsedTime = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 8: Remaining Time present
        if ((flags & 0x100) != 0 && offset + 2 <= data.Length) {
            stepClimberData.RemainingTime = BitConverter.ToUInt16(data, offset);
        }

        StepClimberDataChanged?.Invoke(this, stepClimberData);
    }
}
