using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

internal static class GattServiceExtensions
{
    public static Task<(int, int)> GetSupportedSpeedRangeAsync(this GattDeviceService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedSpeedRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedInclinationRangeAsync(this GattDeviceService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedInclinationRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedResistanceLevelRangeAsync(this GattDeviceService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedResistanceLevelRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedHeartRateRangeAsync(this GattDeviceService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedHeartRateRangeCharacteristicUuid);
    public static Task<(int, int)> GetSupportedPowerRangeAsync(this GattDeviceService service) => ReadRangeAsync(service, FitnessDeviceGuids.SupportedPowerRangeCharacteristicUuid);

    private static async Task<(int, int)> ReadRangeAsync(GattDeviceService service, Guid uuid)
    {
        try {
            var result = await service.GetCharacteristicsForUuidAsync(uuid);
            if (result.Status == GattCommunicationStatus.Success && result.Characteristics.Count > 0) {
                var readResult = await result.Characteristics[0].ReadValueAsync();
                if (readResult.Status == GattCommunicationStatus.Success) {
                    var reader = DataReader.FromBuffer(readResult.Value);
                    return (reader.ReadUInt16(), reader.ReadUInt16());
                }
            }
        }
        catch { /* ignore */ }

        return (0, 0);
    }

    public static async Task<(FitnessMachineFeatures, TargetSettingFeatures)> GetFitnessMachineFeaturesAsync(this GattDeviceService service)
    {
        try {
            var result = await service.GetCharacteristicsForUuidAsync(FitnessDeviceGuids.FitnessMachineFeatureUuid);
            if (result.Status == GattCommunicationStatus.Success && result.Characteristics.Count > 0) {
                var readResult = await result.Characteristics[0].ReadValueAsync();
                if (readResult.Status == GattCommunicationStatus.Success) {
                    var reader = DataReader.FromBuffer(readResult.Value);
                    var fitnessMachineFeatures = (FitnessMachineFeatures)reader.ReadUInt32();
                    var targetSettingFeatures = (TargetSettingFeatures)reader.ReadUInt32();
                    return (fitnessMachineFeatures, targetSettingFeatures);
                }
            }
        }
        catch { /* ignore */ }

        return (FitnessMachineFeatures.None, TargetSettingFeatures.None);
    }
}
