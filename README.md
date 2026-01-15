# ThinkMeta.Devices.Bluetooth

[![NuGet Package](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Core)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core) ThinkMeta.Devices.Bluetooth.Core

[![NuGet Package](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Fitness)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness) ThinkMeta.Devices.Bluetooth.Fitness

[![NuGet Package](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.HeartRate)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate) ThinkMeta.Devices.Bluetooth.HeartRate

ThinkMeta.Devices.Bluetooth is a .NET library for discovering, connecting to, and interacting with Bluetooth fitness and heart rate devices. It provides high-level APIs for scanning, connecting, and controlling devices such as treadmills (FTMS) and heart rate monitors.

## Features

- Scan for Bluetooth FTMS fitness machines (e.g., treadmills)
- Scan for Bluetooth heart rate monitors
- Connect to discovered devices
- Receive real-time data (e.g., speed, distance, heart rate)
- Control supported fitness machines (e.g., set treadmill speed)

## Getting Started

Add the library to your .NET project and use the provided device scanner and device classes to interact with Bluetooth devices.

### Example: Scan and Control a Treadmill (FTMS)

```csharp
var scanner = new FitnessDeviceScanner();
var discoveredDevices = new List<FitnessMachineAdvertisementInfo>();
scanner.DeviceDiscovered += deviceObj => {
    if (deviceObj is FitnessMachineAdvertisementInfo device) {
        // Add device to list and display info
    }
};
scanner.StartScanning();
// Wait for user input, then stop scanning
scanner.StopScanning();

// Connect to a selected device
var treadmill = await FitnessDevice.ConnectAsync(selectedDevice.BluetoothAddress);
treadmill.TreadmillDataChanged += (sender, data) => {
    // Handle treadmill data (speed, distance, etc.)
};
await treadmill.RequestControlAsync();
await treadmill.SetTargetSpeedAsync(1000); // Set speed to 10.00 km/h
```

### Example: Scan and Connect to a Heart Rate Monitor

```csharp
var scanner = new HeartRateMonitorDeviceScanner();
var discoveredDevices = new List<AdvertisementInfo>();
scanner.DeviceDiscovered += device => {
    // Add device to list and display info
};
scanner.StartScanning();
// Wait for user input, then stop scanning
scanner.StopScanning();

// Connect to a selected device
var monitor = await HeartRateMonitorDevice.ConnectAsync(selectedDevice.BluetoothAddress);
monitor.HeartRateMeasurementReceived += hr => Console.WriteLine($"Heart Rate: {hr} bpm");
```

## Requirements

- .NET 10 or later
- Bluetooth adapter supported by your platform

## License

See [LICENSE](LICENSE) for details.
