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

    /// <summary>
    /// Parses step climber data from a GATT characteristic byte array.
    /// </summary>
    /// <param name="data">The raw byte data from the Step Climber Data characteristic.</param>
    /// <returns>The parsed step climber data, or <see langword="null"/> if the data is too short.</returns>
    public static StepClimberData? Parse(byte[] data)
    {
        if (data.Length < 2)
            return null;

        var flags = BitConverter.ToUInt16(data, 0);
        var offset = 2;
        var result = new StepClimberData();

        // Bit 0: More Data (0 = Floors and Step Count present)
        if ((flags & 0x01) == 0 && offset + 4 <= data.Length) { result.Floors = BitConverter.ToUInt16(data, offset); result.StepCount = BitConverter.ToUInt16(data, offset + 2); offset += 4; }
        // Bit 1: Steps Per Minute Present
        if ((flags & 0x02) != 0 && offset + 2 <= data.Length) { result.StepsPerMinute = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 2: Average Step Rate Present
        if ((flags & 0x04) != 0 && offset + 2 <= data.Length) { result.AverageStepRate = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 3: Positive Elevation Gain Present
        if ((flags & 0x08) != 0 && offset + 2 <= data.Length) { result.PositiveElevationGain = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 4: Expended Energy Present
        if ((flags & 0x10) != 0 && offset + 5 <= data.Length) { result.TotalEnergy = BitConverter.ToUInt16(data, offset); result.EnergyPerHour = BitConverter.ToUInt16(data, offset + 2); result.EnergyPerMinute = data[offset + 4]; offset += 5; }
        // Bit 5: Heart Rate Present
        if ((flags & 0x20) != 0 && offset + 1 <= data.Length) { result.HeartRate = data[offset]; offset += 1; }
        // Bit 6: Metabolic Equivalent Present
        if ((flags & 0x40) != 0 && offset + 1 <= data.Length) { result.MetabolicEquivalent = data[offset]; offset += 1; }
        // Bit 7: Elapsed Time Present
        if ((flags & 0x80) != 0 && offset + 2 <= data.Length) { result.ElapsedTime = BitConverter.ToUInt16(data, offset); offset += 2; }
        // Bit 8: Remaining Time Present
        if ((flags & 0x100) != 0 && offset + 2 <= data.Length) { result.RemainingTime = BitConverter.ToUInt16(data, offset); }

        return result;
    }
}
