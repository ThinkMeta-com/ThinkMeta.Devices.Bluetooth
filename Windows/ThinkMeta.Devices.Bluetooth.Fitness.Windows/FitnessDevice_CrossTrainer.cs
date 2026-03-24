using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new cross trainer data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, CrossTrainerData>? CrossTrainerDataChanged;

    private void OnCrossTrainerDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var crossTrainerData = CrossTrainerData.Parse(GetBytes(args));
        if (crossTrainerData is not null)
            CrossTrainerDataChanged?.Invoke(this, crossTrainerData);
    }
}


