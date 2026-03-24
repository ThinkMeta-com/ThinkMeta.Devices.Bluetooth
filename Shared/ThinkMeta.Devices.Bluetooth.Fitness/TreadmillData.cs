namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents treadmill measurement data from the FTMS characteristic, with units as defined by the FTMS specification.
/// </summary>
public class TreadmillData
{
    /// <summary>Instantaneous speed (1/100 km per hour).</summary>
    public int? InstantaneousSpeed { get; set; }
    /// <summary>Average speed (1/100 km per hour).</summary>
    public int? AverageSpeed { get; set; }
    /// <summary>Total distance (meters).</summary>
    public int? TotalDistance { get; set; }
    /// <summary>Inclination (1/10 %).</summary>
    public int? Inclination { get; set; }
    /// <summary>Ramp angle (1/10 degree).</summary>
    public int? RampAngle { get; set; }
    /// <summary>Positive elevation gain (1/10 meters).</summary>
    public int? PositiveElevationGain { get; set; }
    /// <summary>Negative elevation gain (1/10 meters).</summary>
    public int? NegativeElevationGain { get; set; }
    /// <summary>Instantaneous pace (seconds per 500 meters).</summary>
    public int? InstantaneousPace { get; set; }
    /// <summary>Average pace (seconds per 500 meters).</summary>
    public int? AveragePace { get; set; }
    /// <summary>Total energy (kcal).</summary>
    public int? TotalEnergy { get; set; }
    /// <summary>Energy per hour (kcal).</summary>
    public int? EnergyPerHour { get; set; }
    /// <summary>Energy per minute (kcal).</summary>
    public int? EnergyPerMinute { get; set; }
    /// <summary>Heart rate (bpm).</summary>
    public int? HeartRate { get; set; }
    /// <summary>Metabolic equivalent (MET).</summary>
    public int? MetabolicEquivalent { get; set; }
    /// <summary>Elapsed time (seconds).</summary>
    public int? ElapsedTime { get; set; }
    /// <summary>Remaining time (seconds).</summary>
    public int? RemainingTime { get; set; }
    /// <summary>Force on belt (Newtons).</summary>
    public int? ForceOnBelt { get; set; }
    /// <summary>Power output (Watts).</summary>
    public int? PowerOutput { get; set; }

    /// <summary>
    /// Parses treadmill data from a GATT characteristic byte array.
    /// </summary>
    /// <param name="data">The raw byte data from the Treadmill Data characteristic.</param>
    /// <returns>The parsed treadmill data, or <see langword="null"/> if the data is too short.</returns>
    public static TreadmillData? Parse(byte[] data)
    {
        if (data.Length < 2)
            return null;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var result = new TreadmillData();

        // Bit 0: More Data (0 = Speed present)
        if ((flags & 0x01) == 0 && offset + 2 <= data.Length) { result.InstantaneousSpeed = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 1: Average Speed Present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) { result.AverageSpeed = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 2: Total Distance Present
        if ((flags & 0x04) != 0 && offset + 3 <= data.Length) { result.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16); offset += 3; }
        // Bit 3: Inclination and Ramp Angle Present
        if ((flags & 0x08) != 0 && offset + 4 <= data.Length) { result.Inclination = BitConverter.ToInt16(data, offset); result.RampAngle = BitConverter.ToInt16(data, offset + 2); offset += 4; }
        // Bit 4: Elevation Gain Present
        if ((flags & 0x10) != 0 && offset + 4 <= data.Length) { result.PositiveElevationGain = BitConverter.ToUInt16(data, offset); result.NegativeElevationGain = BitConverter.ToUInt16(data, offset + 2); offset += 4; }
        // Bit 5: Instantaneous Pace Present
        if ((flags & 0x20) != 0 && offset + 2 <= data.Length) { result.InstantaneousPace = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 6: Average Pace Present
        if ((flags & 0x40) != 0 && offset + 2 <= data.Length) { result.AveragePace = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 7: Expended Energy Present
        if ((flags & 0x80) != 0 && offset + 5 <= data.Length) { result.TotalEnergy = BitConverter.ToUInt16(data, offset); result.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2); result.EnergyPerMinute = data[offset + 4]; offset += 5; }
        // Bit 8: Heart Rate Present
        if ((flags & 0x100) != 0 && offset + 1 <= data.Length) { result.HeartRate = data[offset]; offset += 1; }
        // Bit 9: Metabolic Equivalent Present
        if ((flags & 0x200) != 0 && offset + 1 <= data.Length) { result.MetabolicEquivalent = data[offset]; offset += 1; }
        // Bit 10: Elapsed Time Present
        if ((flags & 0x400) != 0 && offset + 2 <= data.Length) { result.ElapsedTime = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 11: Remaining Time Present
        if ((flags & 0x800) != 0 && offset + 2 <= data.Length) { result.RemainingTime = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 12: Force on Belt and Power Output Present
        if ((flags & 0x1000) != 0 && offset + 4 <= data.Length) { result.ForceOnBelt = BitConverter.ToInt16(data, offset); result.PowerOutput = BitConverter.ToInt16(data, offset + 2); }

        return result;
    }
}
