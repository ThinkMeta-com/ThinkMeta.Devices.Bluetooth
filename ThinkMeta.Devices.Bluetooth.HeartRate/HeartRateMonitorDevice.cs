using ThinkMeta.Devices.Bluetooth.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace ThinkMeta.Devices.Bluetooth.HeartRate;

/// <summary>
/// Represents a Bluetooth LE heart rate monitor device and handles heart rate measurement events.
/// </summary>
public class HeartRateMonitorDevice : IDisposable
{
    /// <summary>
    /// Occurs when a heart rate measurement is received.
    /// </summary>
    public event Action<int>? HeartRateMeasurementReceived;

    /// <summary>
    /// Occurs when the connection status changes.
    /// </summary>
    public event Action<bool>? ConnectionStatusChanged;

    private readonly BluetoothLEDevice _bluetoothLeDevice;
    private readonly GattCharacteristic _heartRateCharacteristic;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeartRateMonitorDevice"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth LE device.</param>
    /// <param name="characteristic">The heart rate GATT characteristic.</param>
    private HeartRateMonitorDevice(BluetoothLEDevice device, GattCharacteristic characteristic)
    {
        _bluetoothLeDevice = device;
        _bluetoothLeDevice.ConnectionStatusChanged += OnConnectionStatusChanged;
        _heartRateCharacteristic = characteristic;
        _heartRateCharacteristic.ValueChanged += OnHeartRateValueChanged;
    }

    /// <summary>
    /// Asynchronously establishes a connection to a Bluetooth heart rate monitor device using the specified Bluetooth
    /// address.
    /// </summary>
    /// <remarks>The returned <see cref="HeartRateMonitorDevice"/> must be disposed when no longer needed to
    /// release system resources. This method attempts to connect to the device, discover the Heart Rate service and
    /// characteristic, and subscribe to heart rate measurement notifications. If any of these steps fail, a <see
    /// cref="DeviceConnectionException"/> is thrown.</remarks>
    /// <param name="bluetoothAddress">The Bluetooth address of the heart rate monitor device to connect to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="HeartRateMonitorDevice"/> instance representing the connected device.</returns>
    /// <exception cref="DeviceConnectionException">Thrown if the connection to the device fails, the heart rate service or characteristic is not found, or if an
    /// error occurs during the connection process.</exception>
    public static async Task<HeartRateMonitorDevice> ConnectAsync(ulong bluetoothAddress)
    {
        BluetoothLEDevice? bluetoothLeDevice = null;
        try {
            bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
            if (bluetoothLeDevice is null)
                throw new DeviceConnectionException("Failed to connect to device.");

            var servicesResult = await bluetoothLeDevice.GetGattServicesForUuidAsync(HeartRateMonitorDeviceGuids.HeartRateServiceUuid);
            if (servicesResult.Status != GattCommunicationStatus.Success || servicesResult.Services.Count == 0)
                throw new DeviceConnectionException("Heart Rate Service not found.");

            var heartRateService = servicesResult.Services[0];
            var characteristicsResult = await heartRateService.GetCharacteristicsForUuidAsync(HeartRateMonitorDeviceGuids.HeartRateMeasurementCharacteristicUuid);
            if (characteristicsResult.Status != GattCommunicationStatus.Success || characteristicsResult.Characteristics.Count == 0)
                throw new DeviceConnectionException("Heart Rate Measurement Characteristic not found.");

            var heartRateCharacteristic = characteristicsResult.Characteristics[0];
            var status = await heartRateCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status != GattCommunicationStatus.Success)
                throw new DeviceConnectionException("Failed to subscribe to Heart Rate Measurement notifications.");

            return new HeartRateMonitorDevice(bluetoothLeDevice, heartRateCharacteristic);
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
    /// Releases the resources used by the <see cref="HeartRateMonitorDevice"/> class.
    /// </summary>
    /// <param name="disposing">True to release managed resources; otherwise, false.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) {
            _bluetoothLeDevice.ConnectionStatusChanged -= OnConnectionStatusChanged;
            _heartRateCharacteristic.ValueChanged -= OnHeartRateValueChanged;
            _bluetoothLeDevice.Dispose();
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

    private void OnHeartRateMeasurementReceived(int heartRate) => HeartRateMeasurementReceived?.Invoke(heartRate);

    private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args) => ConnectionStatusChanged?.Invoke(sender.ConnectionStatus == BluetoothConnectionStatus.Connected);

    private void OnHeartRateValueChanged(GattCharacteristic sender, GattValueChangedEventArgs e)
    {
        var reader = DataReader.FromBuffer(e.CharacteristicValue);
        var data = new byte[e.CharacteristicValue.Length];
        reader.ReadBytes(data);

        if (data.Length < 2)
            return;

        var flags = data[0];
        int heartRateValue;

        if ((flags & 0x01) == 0) {
            // Heart Rate Value Format is uint8
            heartRateValue = data[1];
        }
        else {
            // Heart Rate Value Format is uint16
            heartRateValue = data[1] | (data[2] << 8);
        }

        OnHeartRateMeasurementReceived(heartRateValue);
    }
}
