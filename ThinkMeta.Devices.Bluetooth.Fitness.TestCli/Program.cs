namespace ThinkMeta.Devices.Bluetooth.Fitness.TestCli;

internal static class Program
{
    private static async Task Main()
    {
        var scanner = new FitnessDeviceScanner();
        var discoveredDevices = new List<FitnessMachineAdvertisementInfo>();
        var deviceDiscoveredEvent = new AutoResetEvent(false);

        scanner.DeviceDiscovered += deviceObj => {
            if (deviceObj is FitnessMachineAdvertisementInfo device) {
                lock (discoveredDevices) {
                    if (!discoveredDevices.Any(d => d.BluetoothAddress == device.BluetoothAddress)) {
                        discoveredDevices.Add(device);
                        Console.WriteLine($"[{discoveredDevices.Count}] {device.Name} ({device.BluetoothAddress:X}) Types: {device.MachineTypes}");
                        _ = deviceDiscoveredEvent.Set();
                    }
                }
            }
        };

        Console.WriteLine("Scanning for FTMS treadmill devices (press Enter to stop)...");
        scanner.StartScanning();
        _ = Console.ReadLine();
        scanner.StopScanning();

        if (discoveredDevices.Count == 0) {
            Console.WriteLine("No treadmill devices found.");
            return;
        }

        Console.WriteLine("Select a device to connect:");
        for (var i = 0; i < discoveredDevices.Count; i++) {
            var d = discoveredDevices[i];
            Console.WriteLine($"[{i + 1}] {d.Name} ({d.BluetoothAddress:X}) Types: {d.MachineTypes}");
        }
        Console.Write("Enter device number: ");
        if (!int.TryParse(Console.ReadLine(), out var selectedIndex) || selectedIndex < 1 || selectedIndex > discoveredDevices.Count) {
            Console.WriteLine("Invalid selection.");
            return;
        }
        var selectedDevice = discoveredDevices[selectedIndex - 1];

        FitnessDevice? treadmill = null;
        using var cts = new CancellationTokenSource();
        try {
            Console.WriteLine("Connecting to device...");
            treadmill = await FitnessDevice.ConnectAsync(selectedDevice.BluetoothAddress);
            treadmill.ConnectionStatusChanged += connected => Console.WriteLine(connected ? "Connected." : "Disconnected.");
            treadmill.TreadmillDataChanged += (sender, data) => {
                Console.WriteLine($"Treadmill Data: Speed={data.InstantaneousSpeed}, Distance={data.TotalDistance}, Incline={data.Inclination}, HR={data.HeartRate}, Power={data.PowerOutput}");
            };
            Console.WriteLine("Connected. Ready for treadmill control menu.");

            // Track current speed in 0.01 km/h units (default to 0 if unknown)
            var currentSpeed = 0;
            treadmill.TreadmillDataChanged += (sender, data) => {
                if (data.InstantaneousSpeed.HasValue) {
                    currentSpeed = data.InstantaneousSpeed.Value;
                }
            };

            // Request control before setting speed
            await treadmill.RequestControlAsync();

            var exitMenu = false;
            while (!exitMenu) {
                Console.WriteLine("\nTreadmill Control Menu:");
                Console.WriteLine($"Current speed: {currentSpeed / 100.0:F2} km/h");
                Console.WriteLine("1. Increase speed by 1 km/h");
                Console.WriteLine("2. Decrease speed by 1 km/h");
                Console.WriteLine("3. Exit and disconnect");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input) {
                    case "1":
                        try {
                            var newSpeed = currentSpeed + 100; // +1 km/h = +100 (0.01 km/h units)
                            await treadmill.SetTargetSpeedAsync(newSpeed);
                            Console.WriteLine($"Requested speed: {newSpeed / 100.0:F2} km/h");
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"Failed to increase speed: {ex.Message}");
                        }
                        break;
                    case "2":
                        try {
                            var newSpeed = currentSpeed - 100; // -1 km/h = -100 (0.01 km/h units)
                            if (newSpeed < 0) {
                                newSpeed = 0;
                            }
                            await treadmill.SetTargetSpeedAsync(newSpeed);
                            Console.WriteLine($"Requested speed: {newSpeed / 100.0:F2} km/h");
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"Failed to decrease speed: {ex.Message}");
                        }
                        break;
                    case "3":
                        exitMenu = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
            await cts.CancelAsync();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally {
            treadmill?.Dispose();
            Console.WriteLine("Disconnected and cleaned up.");
        }
    }
}
