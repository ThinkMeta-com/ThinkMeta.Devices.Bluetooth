namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Bitfield flags for supported FTMS target setting features.
/// </summary>
[Flags]
public enum TargetSettingFeatures : uint
{
    /// <summary>No target setting features supported.</summary>
    None = 0x00000000,
    /// <summary>Speed target setting supported.</summary>
    SpeedTargetSettingSupported = 0x00000001,
    /// <summary>Inclination target setting supported.</summary>
    InclinationTargetSettingSupported = 0x00000002,
    /// <summary>Resistance target setting supported.</summary>
    ResistanceTargetSettingSupported = 0x00000004,
    /// <summary>Power target setting supported.</summary>
    PowerTargetSettingSupported = 0x00000008,
    /// <summary>Heart rate target setting supported.</summary>
    HeartRateTargetSettingSupported = 0x00000010,
    /// <summary>Targeted expended energy configuration supported.</summary>
    TargetedExpendedEnergyConfigurationSupported = 0x00000020,
    /// <summary>Targeted step number configuration supported.</summary>
    TargetedStepNumberConfigurationSupported = 0x00000040,
    /// <summary>Targeted stride number configuration supported.</summary>
    TargetedStrideNumberConfigurationSupported = 0x00000080,
    /// <summary>Targeted distance configuration supported.</summary>
    TargetedDistanceConfigurationSupported = 0x00000100,
    /// <summary>Targeted training time configuration supported.</summary>
    TargetedTrainingTimeConfigurationSupported = 0x00000200,
    /// <summary>Targeted time in two heart rate zones configuration supported.</summary>
    TargetedTimeInTwoHeartRateZonesConfigurationSupported = 0x00000400,
    /// <summary>Targeted time in three heart rate zones configuration supported.</summary>
    TargetedTimeInThreeHeartRateZonesConfigurationSupported = 0x00000800,
    /// <summary>Targeted time in five heart rate zones configuration supported.</summary>
    TargetedTimeInFiveHeartRateZonesConfigurationSupported = 0x00001000,
    /// <summary>Indoor bike simulation parameters supported.</summary>
    IndoorBikeSimulationParametersSupported = 0x00002000,
    /// <summary>Wheel circumference configuration supported.</summary>
    WheelCircumferenceConfigurationSupported = 0x00004000,
    /// <summary>Spin down control supported.</summary>
    SpinDownControlSupported = 0x00008000,
    /// <summary>Targeted cadence configuration supported.</summary>
    TargetedCadenceConfigurationSupported = 0x00010000
}