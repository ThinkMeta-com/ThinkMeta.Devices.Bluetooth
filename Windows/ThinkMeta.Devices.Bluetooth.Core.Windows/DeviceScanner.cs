using Windows.Devices.Bluetooth.Advertisement;

namespace ThinkMeta.Devices.Bluetooth.Core.Windows;

/// <summary>
/// Base class for scanning Bluetooth LE devices by service UUID.
/// </summary>
public abstract class DeviceScanner : DeviceScannerBase<ulong, AdvertisementInfo>
{
    private BluetoothLEAdvertisementWatcher? _watcher;

    /// <summary>
    /// Starts scanning for Bluetooth LE devices advertising the specified service UUID.
    /// </summary>
    /// <param name="serviceUuid">The service UUID to filter devices.</param>
    protected void StartScanning(Guid serviceUuid)
    {
        if (IsScanning)
            return;
        IsScanning = true;
        _watcher = new BluetoothLEAdvertisementWatcher { ScanningMode = BluetoothLEScanningMode.Active };
        _watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(serviceUuid);
        _watcher.Received += OnWatcherReceived;
        _watcher.Start();
        StartDeviceLostTimer();
    }

    /// <summary>
    /// Stops scanning for Bluetooth LE devices.
    /// </summary>
    public void StopScanning()
    {
        if (!IsScanning)
            return;
        IsScanning = false;
        if (_watcher is not null) {
            _watcher.Stop();
            _watcher.Received -= OnWatcherReceived;
            _watcher = null;
        }
        StopDeviceLostTimer();
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

    private void OnWatcherReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        OnAdvertisementReceived(args.BluetoothAddress, args.RawSignalStrengthInDBm, () => CreateAdvertisementInfo(args));
    }
}
