namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new step climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StepClimberData>? StepClimberDataChanged;

    private void OnStepClimberDataReceived(byte[] data)
    {
        var stepClimberData = StepClimberData.Parse(data);
        if (stepClimberData is not null)
            StepClimberDataChanged?.Invoke(this, stepClimberData);
    }
}


