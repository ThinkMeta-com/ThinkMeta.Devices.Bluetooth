using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new stair climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StairClimberData>? StairClimberDataChanged;

    private void OnStairClimberDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var stairClimberData = StairClimberData.Parse(GetBytes(args));
        if (stairClimberData is not null)
            StairClimberDataChanged?.Invoke(this, stairClimberData);
    }
}


