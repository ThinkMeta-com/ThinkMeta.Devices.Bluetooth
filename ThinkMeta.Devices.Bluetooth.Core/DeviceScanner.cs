using System.Collections.Concurrent;
using Windows.Devices.Bluetooth.Advertisement;

namespace ThinkMeta.Devices.Bluetooth.Core;

/// <summary>
/// Base class for scanning Bluetooth LE devices by service UUID.
/// </summary>
public abstract class DeviceScanner
{
    private bool _isScanning;
    private BluetoothLEAdvertisementWatcher? _watcher;
    private Timer? _deviceLostTimer;
    private readonly TimeSpan _deviceLostCheckInterval = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _deviceLostTimeout = TimeSpan.FromSeconds(3);
    private readonly ConcurrentDictionary<ulong, AdvertisementInfo> _devices = new();

    /// <summary>
    /// Occurs when a new Bluetooth device is discovered.
    /// </summary>
    public event Action<AdvertisementInfo>? DeviceDiscovered;

    /// <summary>
    /// Occurs when an existing Bluetooth device is updated.
    /// </summary>
    public event Action<AdvertisementInfo>? DeviceUpdated;

    /// <summary>
    /// Occurs when a previously discovered Bluetooth device is lost.
    /// </summary>
    public event Action<AdvertisementInfo>? DeviceLost;

    /// <summary>
    /// Starts scanning for Bluetooth LE devices advertising the specified service UUID.
    /// </summary>
    /// <param name="serviceUuid">The service UUID to filter devices.</param>
    protected void StartScanning(Guid serviceUuid)
    {
        if (_isScanning)
            return;
        _isScanning = true;
        _watcher = new BluetoothLEAdvertisementWatcher { ScanningMode = BluetoothLEScanningMode.Active };
        _watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(serviceUuid);
        _watcher.Received += OnAdvertisementReceived;
        _watcher.Start();
        _deviceLostTimer = new Timer(CheckForLostDevices, null, _deviceLostCheckInterval, _deviceLostCheckInterval);
    }

    /// <summary>
    /// Stops scanning for Bluetooth LE devices.
    /// </summary>
    public void StopScanning()
    {
        if (!_isScanning)
            return;
        _isScanning = false;
        if (_watcher is not null) {
            _watcher.Stop();
            _watcher.Received -= OnAdvertisementReceived;
            _watcher = null;
        }
        _deviceLostTimer?.Dispose();
        _deviceLostTimer = null;
    }

    /// <summary>
    /// Creates a <see cref="AdvertisementInfo"/> instance from the received Bluetooth LE advertisement.
    /// </summary>
    /// <param name="args">The advertisement event arguments.</param>
    /// <returns>A new <see cref="AdvertisementInfo"/> with address, name, and last seen timestamp.</returns>
    protected virtual AdvertisementInfo CreateAdvertisementInfo(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        return new AdvertisementInfo {
            BluetoothAddress = args.BluetoothAddress,
            Name = args.Advertisement.LocalName ?? string.Empty,
            LastSeen = DateTimeOffset.UtcNow
        };
    }

    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var now = DateTimeOffset.UtcNow;
        var isNew = false;
        var device = _devices.GetOrAdd(args.BluetoothAddress, addr => {
            isNew = true;
            return CreateAdvertisementInfo(args);
        });

        device.AddRssiSample(args.RawSignalStrengthInDBm, now);
        if (isNew)
            DeviceDiscovered?.Invoke(device);
        else
            DeviceUpdated?.Invoke(device);
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