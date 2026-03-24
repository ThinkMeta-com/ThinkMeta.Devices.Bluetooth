namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new rower data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, RowerData>? RowerDataChanged;

    private void OnRowerDataReceived(byte[] data)
    {
        var rowerData = RowerData.Parse(data);
        if (rowerData is not null)
            RowerDataChanged?.Invoke(this, rowerData);
    }
}


