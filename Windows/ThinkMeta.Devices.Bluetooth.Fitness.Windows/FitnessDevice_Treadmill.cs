using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new treadmill data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, TreadmillData>? TreadmillDataChanged;

    private void OnTreadmillDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var treadmillData = TreadmillData.Parse(GetBytes(args));
        if (treadmillData is not null)
            TreadmillDataChanged?.Invoke(this, treadmillData);
    }
}


