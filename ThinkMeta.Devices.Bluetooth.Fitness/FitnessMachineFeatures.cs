namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Bitfield flags for supported FTMS machine features.
/// </summary>
[Flags]
public enum FitnessMachineFeatures : uint
{
    /// <summary>No features supported.</summary>
    None = 0x00000000,
    /// <summary>Average speed supported.</summary>
    AverageSpeedSupported = 0x00000001,
    /// <summary>Cadence supported.</summary>
    CadenceSupported = 0x00000002,
    /// <summary>Total distance supported.</summary>
    TotalDistanceSupported = 0x00000004,
    /// <summary>Inclination supported.</summary>
    InclinationSupported = 0x00000008,
    /// <summary>Elevation gain supported.</summary>
    ElevationGainSupported = 0x00000010,
    /// <summary>Pace supported.</summary>
    PaceSupported = 0x00000020,
    /// <summary>Step count supported.</summary>
    StepCountSupported = 0x00000040,
    /// <summary>Resistance level supported.</summary>
    ResistanceLevelSupported = 0x00000080,
    /// <summary>Stride count supported.</summary>
    StrideCountSupported = 0x00000100,
    /// <summary>Expended energy supported.</summary>
    ExpendedEnergySupported = 0x00000200,
    /// <summary>Heart rate measurement supported.</summary>
    HeartRateMeasurementSupported = 0x00000400,
    /// <summary>Metabolic equivalent supported.</summary>
    MetabolicEquivalentSupported = 0x00000800,
    /// <summary>Elapsed time supported.</summary>
    ElapsedTimeSupported = 0x00001000,
    /// <summary>Remaining time supported.</summary>
    RemainingTimeSupported = 0x00002000,
    /// <summary>Power measurement supported.</summary>
    PowerMeasurementSupported = 0x00004000,
    /// <summary>Force on belt and power output supported.</summary>
    ForceOnBeltAndPowerOutputSupported = 0x00008000,
    /// <summary>User data retention supported.</summary>
    UserDataRetentionSupported = 0x00010000
}
