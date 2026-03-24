namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new stair climber data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, StairClimberData>? StairClimberDataChanged;

    private void OnStairClimberDataReceived(byte[] data)
    {
        var stairClimberData = StairClimberData.Parse(data);
        if (stairClimberData is not null)
            StairClimberDataChanged?.Invoke(this, stairClimberData);
    }
}


