using Plugin.BLE.Abstractions.Contracts;

namespace ThinkMeta.Devices.Bluetooth.Fitness.Maui;

internal static class GattServiceExtensions
{
    public static Task<(int, int)> GetSupportedSpeedRangeAsync(this IService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedSpeedRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedInclinationRangeAsync(this IService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedInclinationRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedResistanceLevelRangeAsync(this IService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedResistanceLevelRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedHeartRateRangeAsync(this IService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedHeartRateRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedPowerRangeAsync(this IService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedPowerRangeCharacteristicUuid);

    private static async Task<(int, int)> ReadRangeAsync(IService service, Guid uuid)
    {
        try {
            var characteristic = await service.GetCharacteristicAsync(uuid);
            if (characteristic is not null) {
                var (data, _) = await characteristic.ReadAsync();
                if (data is { Length: >= 4 })
                    return (BitConverter.ToUInt16(data, 0), BitConverter.ToUInt16(data, 2));
            }
        }
        catch { /* ignore */ }

        return (0, 0);
    }

    public static async Task<(FitnessMachineFeatures, TargetSettingFeatures)> GetFitnessMachineFeaturesAsync(this IService service)
    {
        try {
            var characteristic = await service.GetCharacteristicAsync(FitnessDeviceGuids.FitnessMachineFeatureUuid);
            if (characteristic is not null) {
                var (data, _) = await characteristic.ReadAsync();
                if (data is { Length: >= 8 }) {
                    var fitnessMachineFeatures = (FitnessMachineFeatures)BitConverter.ToUInt32(data, 0);
                    var targetSettingFeatures = (TargetSettingFeatures)BitConverter.ToUInt32(data, 4);
                    return (fitnessMachineFeatures, targetSettingFeatures);
                }
            }
        }
        catch { /* ignore */ }

        return (FitnessMachineFeatures.None, TargetSettingFeatures.None);
    }
}

