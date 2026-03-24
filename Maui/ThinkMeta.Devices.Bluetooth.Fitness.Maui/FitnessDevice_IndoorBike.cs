namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new indoor bike data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, IndoorBikeData>? IndoorBikeDataChanged;

    private void OnIndoorBikeDataReceived(byte[] data)
    {
        var indoorBikeData = IndoorBikeData.Parse(data);
        if (indoorBikeData is not null)
            IndoorBikeDataChanged?.Invoke(this, indoorBikeData);
    }
}


