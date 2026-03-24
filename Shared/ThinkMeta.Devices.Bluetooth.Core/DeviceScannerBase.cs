using System.Collections.Concurrent;

namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Base class for scanning Bluetooth LE devices with device-lost detection.
/// </summary>
/// <typeparam name="TKey">The device identifier type.</typeparam>
/// <typeparam name="TInfo">The advertisement info type.</typeparam>
public abstract class DeviceScannerBase<TKey, TInfo> where TKey : notnull where TInfo : AdvertisementInfoBase
{
    private Timer? _deviceLostTimer;
    private readonly TimeSpan _deviceLostCheckInterval = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _deviceLostTimeout = TimeSpan.FromSeconds(3);
    private readonly ConcurrentDictionary<TKey, TInfo> _devices = new();

    /// <summary>
    /// Occurs when a new Bluetooth device is discovered.
    /// </summary>
    public event Action<TInfo>? DeviceDiscovered;

    /// <summary>
    /// Occurs when an existing Bluetooth device is updated.
    /// </summary>
    public event Action<TInfo>? DeviceUpdated;

    /// <summary>
    /// Occurs when a previously discovered Bluetooth device is lost.
    /// </summary>
    public event Action<TInfo>? DeviceLost;

    /// <summary>
    /// Gets or sets whether scanning is currently active.
    /// </summary>
    protected bool IsScanning { get; set; }

    /// <summary>
    /// Processes a received advertisement, tracking the device and firing discovery/update events.
    /// </summary>
    /// <param name="key">The unique device identifier.</param>
    /// <param name="rssi">The received signal strength.</param>
    /// <param name="factory">Factory to create the advertisement info for new devices.</param>
    protected void OnAdvertisementReceived(TKey key, int rssi, Func<TInfo> factory)
    {
        var isNew = false;
        var device = _devices.GetOrAdd(key, _ => {
            isNew = true;
            return factory();
        });

        device.AddRssiSample(rssi, DateTimeOffset.UtcNow);
        if (isNew)
            DeviceDiscovered?.Invoke(device);
        else
            DeviceUpdated?.Invoke(device);
    }

    /// <summary>
    /// Starts the periodic timer that checks for lost devices.
    /// </summary>
    protected void StartDeviceLostTimer()
    {
        _deviceLostTimer = new Timer(CheckForLostDevices, null, _deviceLostCheckInterval, _deviceLostCheckInterval);
    }

    /// <summary>
    /// Stops and disposes the device-lost timer.
    /// </summary>
    protected void StopDeviceLostTimer()
    {
        _deviceLostTimer?.Dispose();
        _deviceLostTimer = null;
    }

    /// <summary>
    /// Asynchronously stops and disposes the device-lost timer.
    /// </summary>
    protected async Task StopDeviceLostTimerAsync()
    {
        if (_deviceLostTimer is not null)
            await _deviceLostTimer.DisposeAsync();
        _deviceLostTimer = null;
    }

    private void CheckForLostDevices(object? state)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var kvp in _devices) {
            if (now - kvp.Value.LastSeen > _deviceLostTimeout && _devices.TryRemove(kvp.Key, out var lostDevice))
                DeviceLost?.Invoke(lostDevice);
        }
    }
}
