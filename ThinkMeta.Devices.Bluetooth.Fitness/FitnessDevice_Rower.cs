using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents rower measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class RowerData
{
    /// <summary>Stroke rate (strokes/minute).</summary>
    public int? StrokeRate { get; set; }
    /// <summary>Stroke count (count).</summary>
    public int? StrokeCount { get; set; }
    /// <summary>Average stroke rate (strokes/minute).</summary>
    public int? AverageStrokeRate { get; set; }
    /// <summary>Total distance (meters).</summary>
    public int? TotalDistance { get; set; }
    /// <summary>Instantaneous pace (seconds/500m).</summary>
    public int? InstantaneousPace { get; set; }
    /// <summary>Average pace (seconds/500m).</summary>
    public int? AveragePace { get; set; }
    /// <summary>Instantaneous power (Watts).</summary>
    public int? InstantaneousPower { get; set; }
    /// <summary>Average power (Watts).</summary>
    public int? AveragePower { get; set; }
    /// <summary>Resistance level (unitless).</summary>
    public int? ResistanceLevel { get; set; }
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
    /// Occurs when new rower data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, RowerData>? RowerDataChanged;

    private void OnRowerDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        if (data.Length < 2)
            return;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var rowerData = new RowerData();

        // Bit 0: More Data (0 = Stroke Rate present)
        if ((flags & 0x01) == 0 && offset + 3 <= data.Length) {
            rowerData.StrokeRate = data[offset];
            rowerData.StrokeCount = BitConverter.ToUInt16(data, offset + 1);
            offset += 3;
        }

        // Bit 1: Average Stroke Rate
        if ((flags & 0x02) != 0 && offset + 1 <= data.Length) {
            rowerData.AverageStrokeRate = data[offset];
            offset += 1;
        }

        // Bit 2: Total Distance
        if ((flags & 0x04) != 0 && offset + 3 <= data.Length) {
            rowerData.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);
            offset += 3;
        }

        // Bit 3: Instantaneous Pace
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) {
            rowerData.InstantaneousPace = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 4: Average Pace
        if ((flags & 0x10) != 0 && offset + 2 <= data.Length) {
            rowerData.AveragePace = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 5: Instantaneous Power
        if ((flags & 0x20) != 0 && offset + 2 <= data.Length) {
            rowerData.InstantaneousPower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 6: Average Power
        if ((flags & 0x40) != 0 && offset + 2 <= data.Length) {
            rowerData.AveragePower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 7: Resistance Level
        if ((flags & 0x80) != 0 && offset + 1 <= data.Length) {
            rowerData.ResistanceLevel = data[offset];
            offset += 1;
        }

        // Bit 8: Expended Energy
        if ((flags & 0x100) != 0 && offset + 5 <= data.Length) {
            rowerData.TotalEnergy = BitConverter.ToUInt16(data, offset);
            rowerData.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2);
            rowerData.EnergyPerMinute = data[offset + 4];
            offset += 5;
        }

        // Bit 9: Heart Rate
        if ((flags & 0x200) != 0 && offset + 1 <= data.Length) {
            rowerData.HeartRate = data[offset];
            offset += 1;
        }

        // Bit 10: Metabolic Equivalent
        if ((flags & 0x400) != 0 && offset + 1 <= data.Length) {
            rowerData.MetabolicEquivalent = data[offset];
            offset += 1;
        }

        // Bit 11: Elapsed Time
        if ((flags & 0x800) != 0 && offset + 2 <= data.Length) {
            rowerData.ElapsedTime = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 12: Remaining Time
        if ((flags & 0x1000) != 0 && offset + 2 <= data.Length) {
            rowerData.RemainingTime = BitConverter.ToUInt16(data, offset);
        }

        RowerDataChanged?.Invoke(this, rowerData);
    }
}
