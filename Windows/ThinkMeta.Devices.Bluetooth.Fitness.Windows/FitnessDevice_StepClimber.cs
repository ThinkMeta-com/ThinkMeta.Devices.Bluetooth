using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Windows;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new step climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StepClimberData>? StepClimberDataChanged;

    private void OnStepClimberDataChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var stepClimberData = StepClimberData.Parse(GetBytes(args));
        if (stepClimberData is not null)
            StepClimberDataChanged?.Invoke(this, stepClimberData);
    }
}


