using ThinkMeta.Devices.Bluetooth.Core;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Stores information and FTMS device types for a Bluetooth FTMS device.
/// </summary>
public class FitnessMachineAdvertisementInfo : AdvertisementInfo
{
    /// <summary>
    /// Gets or sets the FTMS machine types.
    /// </summary>
    public FitnessMachineTypes MachineTypes { get; internal set; }
}
