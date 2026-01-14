namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Bitfield flags for supported FTMS device types, as defined by the FTMS specification.
/// </summary>
[Flags]
public enum FitnessMachineTypes : ushort
{
    /// <summary>Not Available.</summary>
    None = 0,
    /// <summary>Treadmill supported.</summary>
    Treadmill = 1,
    /// <summary>Cross Trainer supported.</summary>
    CrossTrainer = 2,
    /// <summary>Step Climber supported.</summary>
    StepClimber = 4,
    /// <summary>Stair Climber supported.</summary>
    StairClimber = 8,
    /// <summary>Rower supported.</summary>
    Rower = 16,
    /// <summary>Indoor Bike supported.</summary>
    IndoorBike = 32,
}
