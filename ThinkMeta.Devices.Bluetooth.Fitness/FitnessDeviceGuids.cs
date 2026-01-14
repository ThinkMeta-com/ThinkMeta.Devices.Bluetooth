namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Provides UUIDs for the FTMS Service and its Characteristics.
/// </summary>
public static class FitnessDeviceGuids
{
    /// <summary>
    /// The UUID of the FTMS Service.
    /// </summary>
    public static readonly Guid FtmsServiceUuid = Guid.Parse("00001826-0000-1000-8000-00805f9b34fb"); // FTMS standard UUID

    /// <summary>
    /// The UUID of the Fitness Machine Feature Characteristic.
    /// </summary>
    public static readonly Guid FitnessMachineFeatureUuid = Guid.Parse("00002ACC-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Treadmill Data Characteristic.
    /// </summary>
    public static readonly Guid TreadmillDataCharacteristicUuid = Guid.Parse("00002ACD-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Cross Trainer Data Characteristic.
    /// </summary>
    public static readonly Guid CrossTrainerDataCharacteristicUuid = Guid.Parse("00002ACE-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Step Climber Data Characteristic.
    /// </summary>
    public static readonly Guid StepClimberDataCharacteristicUuid = Guid.Parse("00002ACF-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Stair Climber Data Characteristic.
    /// </summary>
    public static readonly Guid StairClimberDataCharacteristicUuid = Guid.Parse("00002AD0-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Rower Data Characteristic.
    /// </summary>
    public static readonly Guid RowerDataCharacteristicUuid = Guid.Parse("00002AD1-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Indoor Bike Data Characteristic.
    /// </summary>
    public static readonly Guid IndoorBikeDataCharacteristicUuid = Guid.Parse("00002AD2-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Training Status Characteristic.
    /// </summary>
    public static readonly Guid TrainingStatusCharacteristicUuid = Guid.Parse("00002AD3-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Supported Speed Range Characteristic.
    /// </summary>
    public static readonly Guid SupportedSpeedRangeCharacteristicUuid = Guid.Parse("00002AD4-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Supported Inclination Range Characteristic.
    /// </summary>
    public static readonly Guid SupportedInclinationRangeCharacteristicUuid = Guid.Parse("00002AD5-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Supported Resistance Level Range Characteristic.
    /// </summary>
    public static readonly Guid SupportedResistanceLevelRangeCharacteristicUuid = Guid.Parse("00002AD6-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Supported Heart Rate Range Characteristic.
    /// </summary>
    public static readonly Guid SupportedHeartRateRangeCharacteristicUuid = Guid.Parse("00002AD7-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Supported Power Range Characteristic.
    /// </summary>
    public static readonly Guid SupportedPowerRangeCharacteristicUuid = Guid.Parse("00002AD8-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Fitness Machine Control Point Characteristic.
    /// </summary>
    public static readonly Guid FitnessMachineControlPointCharacteristicUuid = Guid.Parse("00002AD9-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// The UUID of the Fitness Machine Status Characteristic.
    /// </summary>
    public static readonly Guid FitnessMachineStatusCharacteristicUuid = Guid.Parse("00002ADA-0000-1000-8000-00805f9b34fb");
}
