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

    /// <summary>
    /// Parses rower data from a GATT characteristic byte array.
    /// </summary>
    /// <param name="data">The raw byte data from the Rower Data characteristic.</param>
    /// <returns>The parsed rower data, or <see langword="null"/> if the data is too short.</returns>
    public static RowerData? Parse(byte[] data)
    {
        if (data.Length < 2)
            return null;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var result = new RowerData();

        // Bit 0: More Data (0 = Stroke Rate present)
        if ((flags & 0x01) == 0 && offset + 3 <= data.Length) { result.StrokeRate = data[offset]; result.StrokeCount = BitConverter.ToUInt16(data, offset + 1); offset += 3; }
        // Bit 1: Average Stroke Rate Present
        if ((flags & 0x02) != 0 && offset + 1 <= data.Length) { result.AverageStrokeRate = data[offset]; offset += 1; }
        // Bit 2: Total Distance Present
        if ((flags & 0x04) != 0 && offset + 3 <= data.Length) { result.TotalDistance = data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16); offset += 3; }
        // Bit 3: Instantaneous Pace Present
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) { result.InstantaneousPace = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 4: Average Pace Present
        if ((flags & 0x10) != 0 && offset + 2 <= data.Length) { result.AveragePace = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 5: Instantaneous Power Present
        if ((flags & 0x20) != 0 && offset + 2 <= data.Length) { result.InstantaneousPower = BitConverter.ToInt16(data, offset); offset += 2; }
        // Bit 6: Average Power Present
        if ((flags & 0x40) != 0 && offset + 2 <= data.Length) { result.AveragePower = BitConverter.ToInt16(data, offset); offset += 2; }
        // Bit 7: Resistance Level Present
        if ((flags & 0x80) != 0 && offset + 1 <= data.Length) { result.ResistanceLevel = data[offset]; offset += 1; }
        // Bit 8: Expended Energy Present
        if ((flags & 0x100) != 0 && offset + 5 <= data.Length) { result.TotalEnergy = BitConverter.ToUInt16(data, offset); result.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2); result.EnergyPerMinute = data[offset + 4]; offset += 5; }
        // Bit 9: Heart Rate Present
        if ((flags & 0x200) != 0 && offset + 1 <= data.Length) { result.HeartRate = data[offset]; offset += 1; }
        // Bit 10: Metabolic Equivalent Present
        if ((flags & 0x400) != 0 && offset + 1 <= data.Length) { result.MetabolicEquivalent = data[offset]; offset += 1; }
        // Bit 11: Elapsed Time Present
        if ((flags & 0x800) != 0 && offset + 2 <= data.Length) { result.ElapsedTime = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 12: Remaining Time Present
        if ((flags & 0x1000) != 0 && offset + 2 <= data.Length) { result.RemainingTime = BitConverter.ToUInt16(data, offset); }

        return result;
    }
}
