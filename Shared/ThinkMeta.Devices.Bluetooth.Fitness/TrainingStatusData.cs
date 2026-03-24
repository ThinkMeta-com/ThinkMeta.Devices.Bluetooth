namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Represents the current training status as defined by the FTMS specification.
/// </summary>
public enum TrainingStatus
{
    /// <summary>Other or unknown status.</summary>
    Other = 0,
    /// <summary>Idle.</summary>
    Idle = 1,
    /// <summary>Warming up.</summary>
    WarmingUp = 2,
    /// <summary>Low intensity interval.</summary>
    LowIntensityInterval = 3,
    /// <summary>High intensity interval.</summary>
    HighIntensityInterval = 4,
    /// <summary>Recovery interval.</summary>
    RecoveryInterval = 5,
    /// <summary>Isometric.</summary>
    Isometric = 6,
    /// <summary>Heart rate control.</summary>
    HeartRateControl = 7,
    /// <summary>Fitness test.</summary>
    FitnessTest = 8,
    /// <summary>Speed outside of control region (low).</summary>
    SpeedOutsideOfControlRegionLow = 9,
    /// <summary>Speed outside of control region (high).</summary>
    SpeedOutsideOfControlRegionHigh = 10,
    /// <summary>Cool down.</summary>
    CoolDown = 11,
    /// <summary>Watt control.</summary>
    WattControl = 12,
    /// <summary>Manual mode.</summary>
    ManualMode = 13,
    /// <summary>Pre-workout.</summary>
    PreWorkout = 14,
    /// <summary>Post-workout.</summary>
    PostWorkout = 15
}

/// <summary>
/// Represents training status data from the FTMS characteristic.
/// </summary>
public class TrainingStatusData
{
    /// <summary>
    /// Gets or sets the current training status.
    /// </summary>
    public TrainingStatus Status { get; set; }
    /// <summary>
    /// Gets or sets the training status text, if present.
    /// </summary>
    public string? Text { get; set; }
}
