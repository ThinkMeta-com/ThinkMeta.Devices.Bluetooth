using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
using IBluetoothAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;
using ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode;

namespace ThinkMeta.Devices.Bluetooth.Core.Maui;

/// <summary>
/// Base class for scanning Bluetooth LE devices by service UUID using Plugin.BLE.
/// </summary>
public abstract class DeviceScanner : DeviceScannerBase<Guid, AdvertisementInfo>
{
    /// <summary>
    /// Gets the Plugin.BLE adapter used for scanning.
    /// </summary>
    protected IBluetoothAdapter Adapter { get; } = CrossBluetoothLE.Current.Adapter;

    /// <summary>
    /// Starts scanning for Bluetooth LE devices advertising the specified service UUID.
    /// </summary>
    /// <param name="serviceUuid">The service UUID to filter devices.</param>
    protected async Task StartScanningAsync(Guid serviceUuid)
    {
        if (IsScanning)
            return;
        IsScanning = true;

        Adapter.DeviceDiscovered += OnDeviceDiscovered;
        Adapter.DeviceAdvertised += OnDeviceDiscovered;
        Adapter.ScanMode = ScanMode.LowLatency;

        StartDeviceLostTimer();

        await Adapter.StartScanningForDevicesAsync([serviceUuid]);
    }

    /// <summary>
    /// Stops scanning for Bluetooth LE devices.
    /// </summary>
    public async Task StopScanningAsync()
    {
        if (!IsScanning)
            return;
        IsScanning = false;

        await Adapter.StopScanningForDevicesAsync();
        Adapter.DeviceDiscovered -= OnDeviceDiscovered;
        Adapter.DeviceAdvertised -= OnDeviceDiscovered;

        await StopDeviceLostTimerAsync();
    }

    /// <summary>
    /// Creates a <see cref="AdvertisementInfo"/> instance from the discovered device.
    /// </summary>
    /// <param name="device">The discovered Plugin.BLE device.</param>
    /// <returns>A new <see cref="AdvertisementInfo"/> with device id, name, and last seen timestamp.</returns>
    protected virtual AdvertisementInfo CreateAdvertisementInfo(Plugin.BLE.Abstractions.Contracts.IDevice device)
    {
        return new AdvertisementInfo {
            DeviceId = device.Id,
            Name = device.Name ?? string.Empty,
            LastSeen = DateTimeOffset.UtcNow,
            Device = device
        };
    }

    private void OnDeviceDiscovered(object? sender, DeviceEventArgs args)
    {
        OnAdvertisementReceived(args.Device.Id, args.Device.Rssi, () => CreateAdvertisementInfo(args.Device));
    }
}

