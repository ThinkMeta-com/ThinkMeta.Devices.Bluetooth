using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents cross trainer measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class CrossTrainerData
{
    /// <summary>Instantaneous speed (1/100 km per hour).</summary>
    public int? InstantaneousSpeed { get; set; }
    /// <summary>Average speed (1/100 km per hour).</summary>
    public int? AverageSpeed { get; set; }
    /// <summary>Total distance (meters).</summary>
    public int? TotalDistance { get; set; }
    /// <summary>Steps per minute (steps/minute).</summary>
    public int? StepsPerMinute { get; set; }
    /// <summary>Average step rate (steps/minute).</summary>
    public int? AverageStepRate { get; set; }
    /// <summary>Stride count (count).</summary>
    public int? StrideCount { get; set; }
    /// <summary>Positive elevation gain (meters).</summary>
    public int? PositiveElevationGain { get; set; }
    /// <summary>Negative elevation gain (meters).</summary>
    public int? NegativeElevationGain { get; set; }
    /// <summary>Inclination (1/10 %).</summary>
    public int? Inclination { get; set; }
    /// <summary>Ramp setting (1/10 degree).</summary>
    public int? RampSetting { get; set; }
    /// <summary>Resistance level (unitless).</summary>
    public int? ResistanceLevel { get; set; }
    /// <summary>Instantaneous power (Watts).</summary>
    public int? InstantaneousPower { get; set; }
    /// <summary>Average power (Watts).</summary>
    public int? AveragePower { get; set; }
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
    /// Occurs when new cross trainer data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, CrossTrainerData>? CrossTrainerDataChanged;

    private void OnCrossTrainerDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        var flags = data[0] | (data[1] << 8) | (data[2] << 16);
        var offset = 3;

        var crossTrainerData = new CrossTrainerData();

        // Bit 0: More Data (0 = Speed present)
        if ((flags & 0x01) == 0 && offset + 2 <= data.Length) {
            crossTrainerData.InstantaneousSpeed = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 1: Average Speed
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.AverageSpeed = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 2: Total Distance
        if ((flags & 0x04) != 0 && offset + 3 <= data.Length) {
            crossTrainerData.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);
            offset += 3;
        }

        // Bit 3: Step Count
        if ((flags & 0x08) != 0 && offset + 4 <= data.Length) {
            crossTrainerData.StepsPerMinute = BitConverter.ToUInt16(data, offset);
            crossTrainerData.AverageStepRate = BitConverter.ToUInt16(data, offset + 2);
            offset += 4;
        }

        // Bit 4: Stride Count
        if ((flags & 0x10) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.StrideCount = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 5: Elevation Gain
        if ((flags & 0x20) != 0 && offset + 4 <= data.Length) {
            crossTrainerData.PositiveElevationGain = BitConverter.ToUInt16(data, offset);
            crossTrainerData.NegativeElevationGain = BitConverter.ToUInt16(data, offset + 2);
            offset += 4;
        }

        // Bit 6: Inclination and Ramp Setting
        if ((flags & 0x40) != 0 && offset + 4 <= data.Length) {
            crossTrainerData.Inclination = BitConverter.ToInt16(data, offset);
            crossTrainerData.RampSetting = BitConverter.ToInt16(data, offset + 2);
            offset += 4;
        }

        // Bit 7: Resistance Level
        if ((flags & 0x80) != 0 && offset + 1 <= data.Length) {
            crossTrainerData.ResistanceLevel = data[offset];
            offset += 1;
        }

        // Bit 8: Instantaneous Power
        if ((flags & 0x100) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.InstantaneousPower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 9: Average Power
        if ((flags & 0x200) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.AveragePower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 10: Expended Energy
        if ((flags & 0x400) != 0 && offset + 5 <= data.Length) {
            crossTrainerData.TotalEnergy = BitConverter.ToUInt16(data, offset);
            crossTrainerData.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2);
            crossTrainerData.EnergyPerMinute = data[offset + 4];
            offset += 5;
        }

        // Bit 11: Heart Rate
        if ((flags & 0x800) != 0 && offset + 1 <= data.Length) {
            crossTrainerData.HeartRate = data[offset];
            offset += 1;
        }

        // Bit 12: Metabolic Equivalent
        if ((flags & 0x1000) != 0 && offset + 1 <= data.Length) {
            crossTrainerData.MetabolicEquivalent = data[offset];
            offset += 1;
        }

        // Bit 13: Elapsed Time
        if ((flags & 0x2000) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.ElapsedTime = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 14: Remaining Time
        if ((flags & 0x4000) != 0 && offset + 2 <= data.Length) {
            crossTrainerData.RemainingTime = BitConverter.ToUInt16(data, offset);
        }

        CrossTrainerDataChanged?.Invoke(this, crossTrainerData);
    }
}
