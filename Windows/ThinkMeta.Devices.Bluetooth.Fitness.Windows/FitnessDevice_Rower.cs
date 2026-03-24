using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new rower data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, RowerData>? RowerDataChanged;

    private void OnRowerDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var rowerData = RowerData.Parse(GetBytes(args));
        if (rowerData is not null)
            RowerDataChanged?.Invoke(this, rowerData);
    }
}


