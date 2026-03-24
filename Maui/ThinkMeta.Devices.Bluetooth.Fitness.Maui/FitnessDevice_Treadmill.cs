namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Occurs when new treadmill data is received from the fitness device.
    /// </summary>
    public event Action<FitnessDevice, TreadmillData>? TreadmillDataChanged;

    private void OnTreadmillDataReceived(byte[] data)
    {
        var treadmillData = TreadmillData.Parse(data);
        if (treadmillData is not null)
            TreadmillDataChanged?.Invoke(this, treadmillData);
    }
}


