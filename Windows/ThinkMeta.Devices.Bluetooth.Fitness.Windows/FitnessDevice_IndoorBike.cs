using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new indoor bike data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, IndoorBikeData>? IndoorBikeDataChanged;

    private void OnIndoorBikeDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var indoorBikeData = IndoorBikeData.Parse(GetBytes(args));
        if (indoorBikeData is not null)
            IndoorBikeDataChanged?.Invoke(this, indoorBikeData);
    }
}


