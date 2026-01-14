using ThinkMeta.Devices.Bluetooth.Core;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Gets the manufacturer name of the device.
    /// </summary>
    public string? ManufacturerName { get; private set; }

    /// <summary>
    /// Gets the model number of the device.
    /// </summary>
    public string? ModelNumber { get; private set; }

    /// <summary>
    /// Gets the serial number of the device.
    /// </summary>
    public string? SerialNumber { get; private set; }

    /// <summary>
    /// Gets the hardware revision of the device.
    /// </summary>
    public string? HardwareRevision { get; private set; }

    /// <summary>
    /// Gets the firmware revision of the device.
    /// </summary>
    public string? FirmwareRevision { get; private set; }

    /// <summary>
    /// Gets the software revision of the device.
    /// </summary>
    public string? SoftwareRevision { get; private set; }

    /// <summary>
    /// Gets the supported speed range.
    /// </summary>
    public (int, int) SupportedSpeedRange { get; private set; }

    /// <summary>
    /// Gets the supported inclination range.
    /// </summary>
    public (int, int) SupportedInclinationRange { get; private set; }

    /// <summary>
    /// Gets the supported resistance level range.
    /// </summary>
    public (int, int) SupportedResistanceLevelRange { get; private set; }

    /// <summary>
    /// Gets the supported heart rate range.
    /// </summary>
    public (int, int) SupportedHeartRateRange { get; private set; }

    /// <summary>
    /// Gets the supported power range.
    /// </summary>
    public (int, int) SupportedPowerRange { get; private set; }

    /// <summary>
    /// Gets the supported FTMS machine features.
    /// </summary>
    public FitnessMachineFeatures FitnessMachineFeatures { get; private set; }

    /// <summary>
    /// Gets the supported FTMS target setting features.
    /// </summary>
    public TargetSettingFeatures TargetSettingFeatures { get; private set; }

    private async Task ReadDeviceInformationAsync(GattDeviceService ftmsService)
    {
        var deviceInformation = await _bluetoothLeDevice.GetDeviceInformationAsync();

        ManufacturerName = deviceInformation.ManufacturerName;
        ModelNumber = deviceInformation.ModelNumber;
        SerialNumber = deviceInformation.SerialNumber;
        HardwareRevision = deviceInformation.HardwareRevision;
        FirmwareRevision = deviceInformation.FirmwareRevision;
        SoftwareRevision = deviceInformation.SoftwareRevision;

        SupportedSpeedRange = await ftmsService.GetSupportedSpeedRangeAsync();
        SupportedInclinationRange = await ftmsService.GetSupportedInclinationRangeAsync();
        SupportedResistanceLevelRange = await ftmsService.GetSupportedResistanceLevelRangeAsync();
        SupportedHeartRateRange = await ftmsService.GetSupportedHeartRateRangeAsync();
        SupportedPowerRange = await ftmsService.GetSupportedPowerRangeAsync();

        (FitnessMachineFeatures, TargetSettingFeatures) = await ftmsService.GetFitnessMachineFeaturesAsync();
    }
}
