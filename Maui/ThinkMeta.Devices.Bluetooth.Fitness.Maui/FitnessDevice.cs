using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
using ThinkMeta.Devices.Bluetooth.Core;
using ThinkMeta.Devices.Bluetooth.Core.Maui;
using IBluetoothAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

/// <summary>
/// Abstract base class for Bluetooth LE FTMS devices and FTMS measurement events.
/// </summary>
public sealed partial class FitnessDevice : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Occurs when the connection status changes.
    /// </summary>
    public event Action<bool>? ConnectionStatusChanged;

    private readonly IBluetoothAdapter _adapter;
    private readonly Plugin.BLE.Abstractions.Contracts.IDevice _device;
    private IReadOnlyList<Plugin.BLE.Abstractions.Contracts.ICharacteristic> _characteristics = [];

    private FitnessDevice(IBluetoothAdapter adapter, Plugin.BLE.Abstractions.Contracts.IDevice device)
    {
        _adapter = adapter;
        _device = device;
        _adapter.DeviceDisconnected += OnDeviceDisconnected;
        _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
    }

    private async Task SetupAsync()
    {
        var ftmsService = await _device.GetServiceAsync(FitnessDeviceGuids.FtmsServiceUuid)
            ?? throw new DeviceConnectionException("FTMS Service not found.");

        await ReadDeviceInformationAsync(ftmsService);

        _characteristics = await ftmsService.GetCharacteristicsAsync();

        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.TreadmillDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.CrossTrainerDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.StepClimberDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.StairClimberDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.RowerDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.IndoorBikeDataCharacteristicUuid, OnCharacteristicValueUpdated);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.TrainingStatusCharacteristicUuid, OnCharacteristicValueUpdated);
    }

    private async Task SubscribeToDataCharacteristicsAsync(Guid uuid, EventHandler<CharacteristicUpdatedEventArgs> callback)
    {
        var characteristic = _characteristics.FirstOrDefault(c => c.Id == uuid);
        if (characteristic is not null) {
            try {
                characteristic.ValueUpdated += callback;
                await characteristic.StartUpdatesAsync();
            }
            catch { /* ignore subscription failures */ }
        }
    }

    private void OnCharacteristicValueUpdated(object? sender, CharacteristicUpdatedEventArgs args)
    {
        var uuid = args.Characteristic.Id;
        var data = args.Characteristic.Value;
        if (data is null || data.Length < 2)
            return;

        if (uuid == FitnessDeviceGuids.TreadmillDataCharacteristicUuid)
            OnTreadmillDataReceived(data);
        else if (uuid == FitnessDeviceGuids.CrossTrainerDataCharacteristicUuid)
            OnCrossTrainerDataReceived(data);
        else if (uuid == FitnessDeviceGuids.StepClimberDataCharacteristicUuid)
            OnStepClimberDataReceived(data);
        else if (uuid == FitnessDeviceGuids.StairClimberDataCharacteristicUuid)
            OnStairClimberDataReceived(data);
        else if (uuid == FitnessDeviceGuids.RowerDataCharacteristicUuid)
            OnRowerDataReceived(data);
        else if (uuid == FitnessDeviceGuids.IndoorBikeDataCharacteristicUuid)
            OnIndoorBikeDataReceived(data);
        else if (uuid == FitnessDeviceGuids.TrainingStatusCharacteristicUuid)
            OnTrainingStatusReceivedAsync(data, args.Characteristic);
    }

    /// <summary>
    /// Connects to an FTMS device and returns the specific device type.
    /// </summary>
    /// <param name="advertisementInfo">The advertisement info of the device.</param>
    /// <returns>The connected FTMS device.</returns>
    public static async Task<FitnessDevice> ConnectAsync(AdvertisementInfo advertisementInfo)
    {
        var adapter = CrossBluetoothLE.Current.Adapter;
        try {
            await adapter.ConnectToDeviceAsync(advertisementInfo.Device);

            var device = new FitnessDevice(adapter, advertisementInfo.Device);
            await device.SetupAsync();

            return device;
        }
        catch (DeviceConnectionException) {
            throw;
        }
        catch (Exception ex) {
            throw new DeviceConnectionException("Unexpected error during connection.", ex);
        }
    }

    /// <summary>
    /// Releases resources used by the device.
    /// </summary>
    /// <param name="disposing">True to release managed resources.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) {
            _adapter.DeviceDisconnected -= OnDeviceDisconnected;
            _adapter.DeviceConnectionLost -= OnDeviceConnectionLost;
            try { _adapter.DisconnectDeviceAsync(_device).GetAwaiter().GetResult(); }
            catch { /* ignore */ }
        }
        _disposed = true;
    }

    /// <summary>
    /// Releases all resources used by the device.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~FitnessDevice()
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
}

