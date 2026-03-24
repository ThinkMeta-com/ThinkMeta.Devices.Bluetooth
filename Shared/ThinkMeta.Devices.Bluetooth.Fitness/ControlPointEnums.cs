namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Spin down control types for FTMS Spin Down Control Point procedure.
/// </summary>
public enum SpinDownControlType : byte
{
    /// <summary>Start the spin down procedure.</summary>
    Start = 0x01,
    /// <summary>Ignore the spin down procedure.</summary>
    Ignore = 0x02
}

/// <summary>
/// Control information for FTMS Stop or Pause Control Point procedure.
/// Only Stop (0x01) and Pause (0x02) are defined by the FTMS specification. Other values are reserved for future use.
/// </summary>
public enum StopOrPauseControlInfo : byte
{
    /// <summary>Stop the training session.</summary>
    Stop = 0x01,
    /// <summary>Pause the training session.</summary>
    Pause = 0x02
}
