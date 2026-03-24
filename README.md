# ThinkMeta.Devices.Bluetooth

[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Core)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core) ThinkMeta.Devices.Bluetooth.Core\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Fitness)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness) ThinkMeta.Devices.Bluetooth.Fitness\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.HeartRate)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate) ThinkMeta.Devices.Bluetooth.HeartRate\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Core.Windows)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core.Windows) ThinkMeta.Devices.Bluetooth.Core.Windows\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Fitness.Windows)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness.Windows) ThinkMeta.Devices.Bluetooth.Fitness.Windows\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.HeartRate.Windows)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate.Windows) ThinkMeta.Devices.Bluetooth.HeartRate.Windows\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Core.Maui)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core.Maui) ThinkMeta.Devices.Bluetooth.Core.Maui\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.Fitness.Maui)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness.Maui) ThinkMeta.Devices.Bluetooth.Fitness.Maui\
[![NuGet](https://img.shields.io/nuget/v/ThinkMeta.Devices.Bluetooth.HeartRate.Maui)](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate.Maui) ThinkMeta.Devices.Bluetooth.HeartRate.Maui

ThinkMeta.Devices.Bluetooth is a .NET library for discovering, connecting to, and interacting with Bluetooth fitness and heart rate devices. It provides high-level APIs for scanning, connecting, and controlling devices such as treadmills (FTMS) and heart rate monitors.

## Features

- Scan for Bluetooth FTMS fitness machines (treadmill, indoor bike, cross trainer, rower, step climber, stair climber)
- Scan for Bluetooth heart rate monitors
- Connect to discovered devices
- Receive real-time data (speed, distance, cadence, power, heart rate, etc.)
- Control supported fitness machines (set speed, inclination, resistance, etc.)
- Shared base classes and parsers for cross-platform reuse

## Packages

| Package | Description |
|---|---|
| [`ThinkMeta.Devices.Bluetooth.Core`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core) | Shared base classes (`AdvertisementInfoBase`, `DeviceScannerBase`) |
| [`ThinkMeta.Devices.Bluetooth.Fitness`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness) | Shared FTMS data models and parsers (`TreadmillData`, `IndoorBikeData`, etc.) |
| [`ThinkMeta.Devices.Bluetooth.HeartRate`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate) | Shared heart rate parsing (`HeartRateMeasurementParser`) |
| [`ThinkMeta.Devices.Bluetooth.Core.Windows`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core.Windows) | Windows BLE scanning and advertisement tracking |
| [`ThinkMeta.Devices.Bluetooth.Fitness.Windows`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness.Windows) | Windows FTMS device connection and control |
| [`ThinkMeta.Devices.Bluetooth.HeartRate.Windows`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate.Windows) | Windows heart rate monitor connection |
| [`ThinkMeta.Devices.Bluetooth.Core.Maui`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Core.Maui) | .NET MAUI BLE scanning via Plugin.BLE |
| [`ThinkMeta.Devices.Bluetooth.Fitness.Maui`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.Fitness.Maui) | .NET MAUI FTMS device connection and control |
| [`ThinkMeta.Devices.Bluetooth.HeartRate.Maui`](https://www.nuget.org/packages/ThinkMeta.Devices.Bluetooth.HeartRate.Maui) | .NET MAUI heart rate monitor connection |

## Getting Started

Install the platform-specific packages for your target. The shared packages are pulled in automatically as transitive dependencies.

### Windows

```
dotnet add package ThinkMeta.Devices.Bluetooth.Fitness.Windows
dotnet add package ThinkMeta.Devices.Bluetooth.HeartRate.Windows
```

### .NET MAUI

```
dotnet add package ThinkMeta.Devices.Bluetooth.Fitness.Maui
dotnet add package ThinkMeta.Devices.Bluetooth.HeartRate.Maui
```

## Examples

### Scan and Control a Treadmill — Windows

```csharp
using ThinkMeta.Devices.Bluetooth.Fitness.Windows;

var scanner = new FitnessDeviceScanner();
scanner.DeviceDiscovered += device => {
    if (device is FitnessMachineAdvertisementInfo ftms)
        Console.WriteLine($"Found: {ftms.Name} ({ftms.MachineTypes})");
};
scanner.StartScanning();

// ... wait for user to pick a device ...
scanner.StopScanning();

// Connect and control
using var treadmill = await FitnessDevice.ConnectAsync(selectedDevice.BluetoothAddress);
treadmill.TreadmillDataChanged += (sender, data) => {
    Console.WriteLine($"Speed: {data.InstantaneousSpeed / 100.0} km/h, Distance: {data.TotalDistance} m");
};
await treadmill.RequestControlAsync();
await treadmill.SetTargetSpeedAsync(1000); // 10.00 km/h
```

### Scan and Control a Treadmill — .NET MAUI

```csharp
using ThinkMeta.Devices.Bluetooth.Fitness.Maui;

var scanner = new FitnessDeviceScanner();
scanner.DeviceDiscovered += device => {
    if (device is FitnessMachineAdvertisementInfo ftms)
        Console.WriteLine($"Found: {ftms.Name} ({ftms.MachineTypes})");
};
await scanner.StartScanningAsync();

// ... wait for user to pick a device ...
await scanner.StopScanningAsync();

// Connect and control
using var treadmill = await FitnessDevice.ConnectAsync(selectedDevice);
treadmill.TreadmillDataChanged += (sender, data) => {
    Console.WriteLine($"Speed: {data.InstantaneousSpeed / 100.0} km/h, Distance: {data.TotalDistance} m");
};
await treadmill.RequestControlAsync();
await treadmill.SetTargetSpeedAsync(1000); // 10.00 km/h
```

### Scan and Connect to a Heart Rate Monitor — Windows

```csharp
using ThinkMeta.Devices.Bluetooth.HeartRate.Windows;

var scanner = new HeartRateMonitorDeviceScanner();
scanner.DeviceDiscovered += device => Console.WriteLine($"Found: {device.Name}");
scanner.StartScanning();

// ... wait for user to pick a device ...
scanner.StopScanning();

using var monitor = await HeartRateMonitorDevice.ConnectAsync(selectedDevice.BluetoothAddress);
monitor.HeartRateMeasurementReceived += hr => Console.WriteLine($"Heart Rate: {hr} bpm");
```

### Scan and Connect to a Heart Rate Monitor — .NET MAUI

```csharp
using ThinkMeta.Devices.Bluetooth.HeartRate.Maui;

var scanner = new HeartRateMonitorDeviceScanner();
scanner.DeviceDiscovered += device => Console.WriteLine($"Found: {device.Name}");
await scanner.StartScanningAsync();

// ... wait for user to pick a device ...
await scanner.StopScanningAsync();

using var monitor = await HeartRateMonitorDevice.ConnectAsync(selectedDevice);
monitor.HeartRateMeasurementReceived += hr => Console.WriteLine($"Heart Rate: {hr} bpm");
```

### Parse FTMS Data Directly (Shared)

The shared data classes include static `Parse()` methods for decoding raw GATT characteristic bytes without a device connection:

```csharp
using ThinkMeta.Devices.Bluetooth.Fitness;

byte[] rawBytes = /* characteristic value */;
var treadmill = TreadmillData.Parse(rawBytes);
if (treadmill is not null)
    Console.WriteLine($"Speed: {treadmill.InstantaneousSpeed / 100.0} km/h");

var bike = IndoorBikeData.Parse(rawBytes);
var rower = RowerData.Parse(rawBytes);
var crossTrainer = CrossTrainerData.Parse(rawBytes);
```

### Parse Heart Rate Data Directly (Shared)

```csharp
using ThinkMeta.Devices.Bluetooth.HeartRate;

byte[] rawBytes = /* characteristic value */;
var heartRate = HeartRateMeasurementParser.Parse(rawBytes);
if (heartRate is not null)
    Console.WriteLine($"Heart Rate: {heartRate} bpm");
```

## Requirements

- .NET 10 or later
- Bluetooth adapter supported by your platform
- .NET MAUI packages require Android, iOS, macOS Catalyst, or Windows

## License

See [LICENSE](LICENSE) for details.
