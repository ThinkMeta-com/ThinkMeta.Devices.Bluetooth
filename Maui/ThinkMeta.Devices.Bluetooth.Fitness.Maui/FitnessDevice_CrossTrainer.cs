namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new cross trainer data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, CrossTrainerData>? CrossTrainerDataChanged;

    private void OnCrossTrainerDataReceived(byte[] data)
    {
        var crossTrainerData = CrossTrainerData.Parse(data);
        if (crossTrainerData is not null)
            CrossTrainerDataChanged?.Invoke(this, crossTrainerData);
    }
}


