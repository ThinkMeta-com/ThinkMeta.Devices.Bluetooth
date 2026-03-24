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

    /// <summary>
    /// Parses cross trainer data from a GATT characteristic byte array.
    /// </summary>
    /// <param name="data">The raw byte data from the Cross Trainer Data characteristic.</param>
    /// <returns>The parsed cross trainer data, or <see langword="null"/> if the data is too short.</returns>
    public static CrossTrainerData? Parse(byte[] data)
    {
        if (data.Length < 3)
            return null;

        var flags = data[0] | (data[1] << 8) | (data[2] << 16);
        var offset = 3;
        var result = new CrossTrainerData();

        // Bit 0: More Data (0 = Speed present)
        if ((flags & 0x01) == 0 && offset + 2 <= data.Length) { result.InstantaneousSpeed = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 1: Average Speed Present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) { result.AverageSpeed = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 2: Total Distance Present
        if ((flags & 0x04) != 0 && offset + 3 <= data.Length) { result.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16); offset += 3; }
        // Bit 3: Step Count Present
        if ((flags & 0x08) != 0 && offset + 4 <= data.Length) { result.StepsPerMinute = BitConverter.ToUInt16(data, offset); result.AverageStepRate = BitConverter.ToUInt16(data, offset + 2); offset += 4; }
        // Bit 4: Stride Count Present
        if ((flags & 0x10) != 0 && offset + 2 <= data.Length) { result.StrideCount = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 5: Elevation Gain Present
        if ((flags & 0x20) != 0 && offset + 4 <= data.Length) { result.PositiveElevationGain = BitConverter.ToUInt16(data, offset); result.NegativeElevationGain = BitConverter.ToUInt16(data, offset + 2); offset += 4; }
        // Bit 6: Inclination and Ramp Setting Present
        if ((flags & 0x40) != 0 && offset + 4 <= data.Length) { result.Inclination = BitConverter.ToInt16(data, offset); result.RampSetting = BitConverter.ToInt16(data, offset + 2); offset += 4; }
        // Bit 7: Resistance Level Present
        if ((flags & 0x80) != 0 && offset + 1 <= data.Length) { result.ResistanceLevel = data[offset]; offset += 1; }
        // Bit 8: Instantaneous Power Present
        if ((flags & 0x100) != 0 && offset + 2 <= data.Length) { result.InstantaneousPower = BitConverter.ToInt16(data, offset); offset += 2; }
        // Bit 9: Average Power Present
        if ((flags & 0x200) != 0 && offset + 2 <= data.Length) { result.AveragePower = BitConverter.ToInt16(data, offset); offset += 2; }
        // Bit 10: Expended Energy Present
        if ((flags & 0x400) != 0 && offset + 5 <= data.Length) { result.TotalEnergy = BitConverter.ToUInt16(data, offset); result.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2); result.EnergyPerMinute = data[offset + 4]; offset += 5; }
        // Bit 11: Heart Rate Present
        if ((flags & 0x800) != 0 && offset + 1 <= data.Length) { result.HeartRate = data[offset]; offset += 1; }
        // Bit 12: Metabolic Equivalent Present
        if ((flags & 0x1000) != 0 && offset + 1 <= data.Length) { result.MetabolicEquivalent = data[offset]; offset += 1; }
        // Bit 13: Elapsed Time Present
        if ((flags & 0x2000) != 0 && offset + 2 <= data.Length) { result.ElapsedTime = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 14: Remaining Time Present
        if ((flags & 0x4000) != 0 && offset + 2 <= data.Length) { result.RemainingTime = BitConverter.ToUInt16(data, offset); }

        return result;
    }
}
