using ThinkMeta.Devices.Bluetooth.Core.Windows;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

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

