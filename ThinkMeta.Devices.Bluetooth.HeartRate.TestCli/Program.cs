using ThinkMeta.Devices.Bluetooth.Core;

namespace ThinkMeta.Devices.Bluetooth.HeartRate.TestCli;

internal static class Program
{
    private static async Task Main()
    {
        var scanner = new HeartRateMonitorDeviceScanner();
        var discoveredDevices = new List<AdvertisementInfo>();
        var deviceDiscoveredEvent = new AutoResetEvent(false);

        scanner.DeviceDiscovered += device => {
            lock (discoveredDevices) {
                if (!discoveredDevices.Any(d => d.BluetoothAddress == device.BluetoothAddress)) {
                    discoveredDevices.Add(device);
                    Console.WriteLine($"[{discoveredDevices.Count}] {device.Name} ({device.BluetoothAddress:X}) RSSI: {device.SmoothedRssi:F1}");
                    _ = deviceDiscoveredEvent.Set();
                }
            }
        };

        Console.WriteLine("Scanning for heart rate devices (press Enter to stop)...");
        scanner.StartScanning();
        _ = Console.ReadLine();
        scanner.StopScanning();

        if (discoveredDevices.Count == 0) {
            Console.WriteLine("No heart rate devices found.");
            return;
        }

        Console.WriteLine("Select a device to connect:");
        for (var i = 0; i < discoveredDevices.Count; i++) {
            var d = discoveredDevices[i];
            Console.WriteLine($"[{i + 1}] {d.Name} ({d.BluetoothAddress:X}) RSSI: {d.SmoothedRssi:F1}");
        }
        Console.Write("Enter device number: ");
        if (!int.TryParse(Console.ReadLine(), out var selectedIndex) || selectedIndex < 1 || selectedIndex > discoveredDevices.Count) {
            Console.WriteLine("Invalid selection.");
            return;
        }
        var selectedDevice = discoveredDevices[selectedIndex - 1];

        HeartRateMonitorDevice? monitor = null;
        try {
            Console.WriteLine("Connecting to device...");
            monitor = await HeartRateMonitorDevice.ConnectAsync(selectedDevice.BluetoothAddress);
            monitor.HeartRateMeasurementReceived += hr => Console.WriteLine($"Heart Rate: {hr} bpm");
            monitor.ConnectionStatusChanged += connected => Console.WriteLine(connected ? "Connected." : "Disconnected.");
            Console.WriteLine("Connected. Listening for heart rate values. Press Enter to disconnect...");
            _ = Console.ReadLine();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally {
            monitor?.Dispose();
            Console.WriteLine("Disconnected and cleaned up.");
        }
    }
}
