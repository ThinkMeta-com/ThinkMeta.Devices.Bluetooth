using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents stair climber measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class StairClimberData
{
    /// <summary>Floors climbed (count).</summary>
    public int? Floors { get; set; }
    /// <summary>Steps per minute (steps/minute).</summary>
    public int? StepsPerMinute { get; set; }
    /// <summary>Average step rate (steps/minute).</summary>
    public int? AverageStepRate { get; set; }
    /// <summary>Positive elevation gain (meters).</summary>
    public int? PositiveElevationGain { get; set; }
    /// <summary>Stride count (count).</summary>
    public int? StrideCount { get; set; }
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
    /// Occurs when new stair climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StairClimberData>? StairClimberDataChanged;

    private void OnStairClimberDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        if (data.Length < 2)
            return;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var stairClimberData = new StairClimberData();

        // Bit 0: More Data (0 = Floors present)
        if ((flags & 0x01) == 0 && offset + 2 <= data.Length) {
            stairClimberData.Floors = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 1: Steps Per Minute present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) {
            stairClimberData.StepsPerMinute = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 2: Average Step Rate present
        if ((flags & 0x04) != 0 && offset + 2 <= data.Length) {
            stairClimberData.AverageStepRate = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 3: Positive Elevation Gain present
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) {
            stairClimberData.PositiveElevationGain = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 4: Stride Count present
        if ((flags & 0x10) != 0 && offset + 2 <= data.Length) {
            stairClimberData.StrideCount = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 5: Energy Data present
        if ((flags & 0x20) != 0 && offset + 5 <= data.Length) {
            stairClimberData.TotalEnergy = BitConverter.ToUInt16(data, offset);
            stairClimberData.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2);
            stairClimberData.EnergyPerMinute = data[offset + 4];
            offset += 5;
        }

        // Bit 6: Heart Rate present
        if ((flags & 0x40) != 0 && offset + 1 <= data.Length) {
            stairClimberData.HeartRate = data[offset];
            offset += 1;
        }

        // Bit 7: Metabolic Equivalent present
        if ((flags & 0x80) != 0 && offset + 1 <= data.Length) {
            stairClimberData.MetabolicEquivalent = data[offset];
            offset += 1;
        }

        // Bit 8: Elapsed Time present
        if ((flags & 0x100) != 0 && offset + 2 <= data.Length) {
            stairClimberData.ElapsedTime = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 9: Remaining Time present
        if ((flags & 0x200) != 0 && offset + 2 <= data.Length) {
            stairClimberData.RemainingTime = BitConverter.ToUInt16(data, offset);
        }

        StairClimberDataChanged?.Invoke(this, stairClimberData);
    }
}
