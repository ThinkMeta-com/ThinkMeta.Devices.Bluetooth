using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
using ThinkMeta.Devices.Bluetooth.Core;
using ThinkMeta.Devices.Bluetooth.Core.Maui;
using IBluetoothAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;

namespace ThinkMeta.Devices.Bluetooth.HeartRate.Maui;

/// <summary>
/// Represents a Bluetooth LE heart rate monitor device and handles heart rate measurement events.
/// </summary>
#pragma warning disable S3453 // Class with only private constructor used via static factory method
public class HeartRateMonitorDevice : IDisposable
#pragma warning restore S3453
{
    /// <summary>
    /// Occurs when a heart rate measurement is received.
    /// </summary>
    public event Action<int>? HeartRateMeasurementReceived;

    /// <summary>
    /// Occurs when the connection status changes.
    /// </summary>
    public event Action<bool>? ConnectionStatusChanged;

    private readonly IBluetoothAdapter _adapter;
    private readonly Plugin.BLE.Abstractions.Contracts.IDevice _device;
    private readonly Plugin.BLE.Abstractions.Contracts.ICharacteristic _heartRateCharacteristic;
    private bool _disposed;

    private HeartRateMonitorDevice(IBluetoothAdapter adapter, Plugin.BLE.Abstractions.Contracts.IDevice device, Plugin.BLE.Abstractions.Contracts.ICharacteristic characteristic)
    {
        _adapter = adapter;
        _device = device;
        _heartRateCharacteristic = characteristic;
        _adapter.DeviceDisconnected += OnDeviceDisconnected;
        _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        _heartRateCharacteristic.ValueUpdated += OnHeartRateValueChanged;
    }

    /// <summary>
    /// Asynchronously establishes a connection to a Bluetooth heart rate monitor device.
    /// </summary>
    /// <param name="advertisementInfo">The advertisement info of the heart rate monitor device to connect to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="HeartRateMonitorDevice"/> instance representing the connected device.</returns>
    /// <exception cref="DeviceConnectionException">Thrown if the connection to the device fails, the heart rate service or characteristic is not found, or if an
    /// error occurs during the connection process.</exception>
    public static async Task<HeartRateMonitorDevice> ConnectAsync(AdvertisementInfo advertisementInfo)
    {
        var adapter = CrossBluetoothLE.Current.Adapter;
        try {
            await adapter.ConnectToDeviceAsync(advertisementInfo.Device);

            var service = await advertisementInfo.Device.GetServiceAsync(HeartRateMonitorDeviceGuids.HeartRateServiceUuid)
                ?? throw new DeviceConnectionException("Heart Rate Service not found.");

            var characteristic = await service.GetCharacteristicAsync(HeartRateMonitorDeviceGuids.HeartRateMeasurementCharacteristicUuid)
                ?? throw new DeviceConnectionException("Heart Rate Measurement Characteristic not found.");

            await characteristic.StartUpdatesAsync();

            return new HeartRateMonitorDevice(adapter, advertisementInfo.Device, characteristic);
        }
        catch (DeviceConnectionException) {
            throw;
        }
        catch (Exception ex) {
            throw new DeviceConnectionException("Unexpected error during connection.", ex);
        }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="HeartRateMonitorDevice"/> class.
    /// </summary>
    /// <param name="disposing">True to release managed resources; otherwise, false.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) {
            _adapter.DeviceDisconnected -= OnDeviceDisconnected;
            _adapter.DeviceConnectionLost -= OnDeviceConnectionLost;
            _heartRateCharacteristic.ValueUpdated -= OnHeartRateValueChanged;
            try { _adapter.DisconnectDeviceAsync(_device).GetAwaiter().GetResult(); }
            catch { /* ignore */ }
        }
        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    ~HeartRateMonitorDevice()
    {
        Dispose(false);
    }

    private void OnDeviceDisconnected(object? sender, DeviceEventArgs args)
    {
        if (args.Device.Id == _device.Id)
            ConnectionStatusChanged?.Invoke(false);
    }

    private void OnDeviceConnectionLost(object? sender, DeviceErrorEventArgs args)
    {
        if (args.Device.Id == _device.Id)
            ConnectionStatusChanged?.Invoke(false);
    }

    private void OnHeartRateValueChanged(object? sender, CharacteristicUpdatedEventArgs args)
    {
        var heartRate = HeartRateMeasurementParser.Parse(args.Characteristic.Value);
        if (heartRate is not null)
            HeartRateMeasurementReceived?.Invoke(heartRate.Value);
    }
}

