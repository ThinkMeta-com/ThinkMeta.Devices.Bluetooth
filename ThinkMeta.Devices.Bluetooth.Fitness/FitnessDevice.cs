using ThinkMeta.Devices.Bluetooth.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

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

    private readonly BluetoothLEDevice _bluetoothLeDevice;
    private IReadOnlyList<GattCharacteristic> _characteristics = null!;

    private FitnessDevice(BluetoothLEDevice device)
    {
        _bluetoothLeDevice = device;
        _bluetoothLeDevice.ConnectionStatusChanged += OnConnectionStatusChanged;
    }

    private async Task SetupAsync()
    {
        var servicesResult = await _bluetoothLeDevice.GetGattServicesForUuidAsync(FitnessDeviceGuids.FtmsServiceUuid);
        if (servicesResult.Status != GattCommunicationStatus.Success || servicesResult.Services.Count == 0)
            throw new DeviceConnectionException("FTMS Service not found.");

        var ftmsService = servicesResult.Services[0];

        await ReadDeviceInformationAsync(ftmsService);

        var characteristicsResult = await ftmsService.GetCharacteristicsAsync();

        if (characteristicsResult.Status != GattCommunicationStatus.Success)
            throw new DeviceConnectionException("Failed to retrieve FTMS characteristics.");

        _characteristics = characteristicsResult.Characteristics;

        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.TreadmillDataCharacteristicUuid, OnTreadmillDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.CrossTrainerDataCharacteristicUuid, OnCrossTrainerDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.StepClimberDataCharacteristicUuid, OnStepClimberDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.StairClimberDataCharacteristicUuid, OnStairClimberDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.RowerDataCharacteristicUuid, OnRowerDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.IndoorBikeDataCharacteristicUuid, OnIndoorBikeDataChanged);
        await SubscribeToDataCharacteristicsAsync(FitnessDeviceGuids.TrainingStatusCharacteristicUuid, OnTrainingStatusChangedAsync);
    }

    private async Task SubscribeToDataCharacteristicsAsync(Guid uuid, TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> callback)
    {
        var characteristic = _characteristics.FirstOrDefault(c => c.Uuid == uuid);
        if (characteristic is not null) {
            try {
                var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status == GattCommunicationStatus.Success) {
                    characteristic.ValueChanged += callback;
                }
            }
            catch { /* ignore subscription failures */ }
        }
    }

    private static byte[] GetBytes(GattValueChangedEventArgs args) => GetBytes(args.CharacteristicValue);

    private static byte[] GetBytes(IBuffer buffer)
    {
        var reader = DataReader.FromBuffer(buffer);
        reader.ByteOrder = ByteOrder.LittleEndian;
        var data = new byte[buffer.Length];
        reader.ReadBytes(data);
        return data;
    }

    /// <summary>
    /// Connects to an FTMS device and returns the specific device type.
    /// </summary>
    /// <param name="bluetoothAddress">The Bluetooth address of the device.</param>
    /// <returns>The connected FTMS device.</returns>
    public static async Task<FitnessDevice> ConnectAsync(ulong bluetoothAddress)
    {
        BluetoothLEDevice? bluetoothLeDevice = null;
        try {
            bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
            if (bluetoothLeDevice is null)
                throw new DeviceConnectionException("Failed to connect to device.");

            var device = new FitnessDevice(bluetoothLeDevice);
            await device.SetupAsync();

            return device;
        }
        catch (DeviceConnectionException) {
            bluetoothLeDevice?.Dispose();
            throw;
        }
        catch (Exception ex) {
            bluetoothLeDevice?.Dispose();
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
            _bluetoothLeDevice.ConnectionStatusChanged -= OnConnectionStatusChanged;
            _bluetoothLeDevice.Dispose();
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

    private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args) => ConnectionStatusChanged?.Invoke(sender.ConnectionStatus == BluetoothConnectionStatus.Connected);
}
