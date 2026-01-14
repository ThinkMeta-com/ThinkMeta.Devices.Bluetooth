using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents indoor bike measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class IndoorBikeData
{
    /// <summary>Instantaneous speed (km/h).</summary>
    public int? InstantaneousSpeed { get; set; }
    /// <summary>Average speed (km/h).</summary>
    public int? AverageSpeed { get; set; }
    /// <summary>Instantaneous cadence (0.5 revolutions/minute).</summary>
    public int? InstantaneousCadence { get; set; }
    /// <summary>Average cadence (0.5 revolutions/minute).</summary>
    public int? AverageCadence { get; set; }
    /// <summary>Total distance (meters).</summary>
    public int? TotalDistance { get; set; }
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
    /// Occurs when new indoor bike data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, IndoorBikeData>? IndoorBikeDataChanged;

    private void OnIndoorBikeDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var data = GetBytes(args);

        if (data.Length < 2)
            return;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var indoorBikeData = new IndoorBikeData();

        // Bit 0: More Data (0 = Speed present)
        if ((flags & 0x01) == 0 && offset + 2 <= data.Length) {
            indoorBikeData.InstantaneousSpeed = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 1: Average Speed Present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.AverageSpeed = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 2: Instantaneous Cadence Present
        if ((flags & 0x04) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.InstantaneousCadence = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 3: Average Cadence Present
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.AverageCadence = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 4: Total Distance Present
        if ((flags & 0x10) != 0 && offset + 3 <= data.Length) {
            indoorBikeData.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);
            offset += 3;
        }

        // Bit 5: Resistance Level Present
        if ((flags & 0x20) != 0 && offset + 1 <= data.Length) {
            indoorBikeData.ResistanceLevel = data[offset];
            offset += 1;
        }

        // Bit 6: Instantaneous Power Present
        if ((flags & 0x40) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.InstantaneousPower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 7: Average Power Present
        if ((flags & 0x80) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.AveragePower = BitConverter.ToInt16(data, offset);
            offset += 2;
        }

        // Bit 8: Expended Energy Present
        if ((flags & 0x100) != 0 && offset + 5 <= data.Length) {
            indoorBikeData.TotalEnergy = BitConverter.ToUInt16(data, offset);
            indoorBikeData.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2);
            indoorBikeData.EnergyPerMinute = data[offset + 4];
            offset += 5;
        }

        // Bit 9: Heart Rate Present
        if ((flags & 0x200) != 0 && offset + 1 <= data.Length) {
            indoorBikeData.HeartRate = data[offset];
            offset += 1;
        }

        // Bit 10: Metabolic Equivalent Present
        if ((flags & 0x400) != 0 && offset + 1 <= data.Length) {
            indoorBikeData.MetabolicEquivalent = data[offset];
            offset += 1;
        }

        // Bit 11: Elapsed Time Present
        if ((flags & 0x800) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.ElapsedTime = BitConverter.ToUInt16(data, offset);
            offset += 2;
        }

        // Bit 12: Remaining Time Present
        if ((flags & 0x1000) != 0 && offset + 2 <= data.Length) {
            indoorBikeData.RemainingTime = BitConverter.ToUInt16(data, offset);
        }

        IndoorBikeDataChanged?.Invoke(this, indoorBikeData);
    }
}
