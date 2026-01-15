using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace ThinkMeta.Devices.Bluetooth.Fitness;

/// <summary>
/// Spin down control types for FTMS Spin Down Control Point procedure.
/// </summary>
public enum SpinDownControlType : byte
{
    /// <summary>Start the spin down procedure.</summary>
    Start = 0x01,
    /// <summary>Ignore the spin down procedure.</summary>
    Ignore = 0x02
}

/// <summary>
/// Control information for FTMS Stop or Pause Control Point procedure.
/// Only Stop (0x01) and Pause (0x02) are defined by the FTMS specification. Other values are reserved for future use.
/// </summary>
public enum StopOrPauseControlInfo : byte
{
    /// <summary>Stop the training session.</summary>
    Stop = 0x01,
    /// <summary>Pause the training session.</summary>
    Pause = 0x02
}

public sealed partial class FitnessDevice
{
    /// <summary>
    /// Requests control of the fitness machine.
    /// </summary>
    public Task RequestControlAsync() => WriteControlPointAsync(0x00);

    /// <summary>
    /// Resets the controllable settings of the fitness machine.
    /// </summary>
    public Task ResetAsync() => WriteControlPointAsync(0x01);

    /// <summary>
    /// Sets the target speed in 0.01 km/h units.
    /// </summary>
    /// <param name="speed">Target speed (0-65535, 0.01 km/h units).</param>
    public Task SetTargetSpeedAsync(int speed)
        => WriteControlPointWithPayloadAsync(0x02, BitConverter.GetBytes((ushort)(speed is >= 0 and <= 0xFFFF ? speed : throw new ArgumentOutOfRangeException(nameof(speed)))));

    /// <summary>
    /// Sets the target inclination in 0.1% units.
    /// </summary>
    /// <param name="inclination">Target inclination (short.MinValue to short.MaxValue, 0.1% units).</param>
    public Task SetTargetInclinationAsync(int inclination)
        => WriteControlPointWithPayloadAsync(0x03, BitConverter.GetBytes((short)(inclination is >= short.MinValue and <= short.MaxValue ? inclination : throw new ArgumentOutOfRangeException(nameof(inclination)))));

    /// <summary>
    /// Sets the target resistance level (0.1 units).
    /// </summary>
    /// <param name="resistance">Target resistance (0-255, 0.1 units).</param>
    public Task SetTargetResistanceLevelAsync(int resistance)
        => WriteControlPointWithPayloadAsync(0x04, [(byte)(resistance is >= 0 and <= 0xFF ? resistance : throw new ArgumentOutOfRangeException(nameof(resistance)))]);

    /// <summary>
    /// Sets the target power in watts.
    /// </summary>
    /// <param name="power">Target power (short.MinValue to short.MaxValue, watts).</param>
    public Task SetTargetPowerAsync(int power)
        => WriteControlPointWithPayloadAsync(0x05, BitConverter.GetBytes((short)(power is >= short.MinValue and <= short.MaxValue ? power : throw new ArgumentOutOfRangeException(nameof(power)))));

    /// <summary>
    /// Sets the target heart rate in BPM.
    /// </summary>
    /// <param name="heartRate">Target heart rate (0-255, BPM).</param>
    public Task SetTargetHeartRateAsync(int heartRate)
        => WriteControlPointWithPayloadAsync(0x06, [(byte)(heartRate is >= 0 and <= 0xFF ? heartRate : throw new ArgumentOutOfRangeException(nameof(heartRate)))]);

    /// <summary>
    /// Starts or resumes a training session.
    /// </summary>
    public Task StartOrResumeAsync() => WriteControlPointAsync(0x07);

    /// <summary>
    /// Stops or pauses a training session.
    /// </summary>
    /// <param name="controlInfo">Stop or pause control info.</param>
    public Task StopOrPauseAsync(StopOrPauseControlInfo controlInfo)
        => WriteControlPointWithPayloadAsync(0x08, [(byte)controlInfo]);

    /// <summary>
    /// Sets the targeted expended energy in calories.
    /// </summary>
    /// <param name="calories">Targeted expended energy (0-65535, calories).</param>
    public Task SetTargetedExpendedEnergyAsync(int calories)
        => WriteControlPointWithPayloadAsync(0x09, BitConverter.GetBytes((ushort)(calories is >= 0 and <= 0xFFFF ? calories : throw new ArgumentOutOfRangeException(nameof(calories)))));

    /// <summary>
    /// Sets the targeted number of steps.
    /// </summary>
    /// <param name="steps">Targeted number of steps (0-65535).</param>
    public Task SetTargetedNumberOfStepsAsync(int steps)
        => WriteControlPointWithPayloadAsync(0x0A, BitConverter.GetBytes((ushort)(steps is >= 0 and <= 0xFFFF ? steps : throw new ArgumentOutOfRangeException(nameof(steps)))));

    /// <summary>
    /// Sets the targeted number of strides.
    /// </summary>
    /// <param name="strides">Targeted number of strides (0-65535).</param>
    public Task SetTargetedNumberOfStridesAsync(int strides)
        => WriteControlPointWithPayloadAsync(0x0B, BitConverter.GetBytes((ushort)(strides is >= 0 and <= 0xFFFF ? strides : throw new ArgumentOutOfRangeException(nameof(strides)))));

    /// <summary>
    /// Sets the targeted distance in meters.
    /// </summary>
    /// <param name="meters">Targeted distance (0-16777215, meters).</param>
    public Task SetTargetedDistanceAsync(int meters)
        => WriteControlPointWithPayloadAsync(0x0C, meters is >= 0 and <= 0xFFFFFF
            ? [(byte)(meters & 0xFF), (byte)((meters >> 8) & 0xFF), (byte)((meters >> 16) & 0xFF)]
            : throw new ArgumentOutOfRangeException(nameof(meters)));

    /// <summary>
    /// Sets the targeted training time in seconds.
    /// </summary>
    /// <param name="seconds">Targeted training time (0-65535, seconds).</param>
    public Task SetTargetedTrainingTimeAsync(int seconds)
        => WriteControlPointWithPayloadAsync(0x0D, BitConverter.GetBytes((ushort)(seconds is >= 0 and <= 0xFFFF ? seconds : throw new ArgumentOutOfRangeException(nameof(seconds)))));

    /// <summary>
    /// Sets the targeted time in two heart rate zones (Fat Burn and Fitness) in seconds.
    /// </summary>
    /// <param name="timeInFatBurnZone">Time in Fat Burn Zone (seconds, 0-65535).</param>
    /// <param name="timeInFitnessZone">Time in Fitness Zone (seconds, 0-65535).</param>
    public Task SetTargetedTimeInTwoHeartRateZonesAsync(int timeInFatBurnZone, int timeInFitnessZone)
        => WriteControlPointWithPayloadAsync(0x0E, [
            (byte)(timeInFatBurnZone is >= 0 and <= 0xFFFF ? timeInFatBurnZone : throw new ArgumentOutOfRangeException(nameof(timeInFatBurnZone))),
            (byte)((timeInFatBurnZone >> 8) & 0xFF),
            (byte)(timeInFitnessZone is >= 0 and <= 0xFFFF ? timeInFitnessZone : throw new ArgumentOutOfRangeException(nameof(timeInFitnessZone))),
            (byte)((timeInFitnessZone >> 8) & 0xFF)
        ]);

    /// <summary>
    /// Sets the targeted time in three heart rate zones (Light, Moderate, Hard) in seconds.
    /// </summary>
    /// <param name="timeInLightZone">Time in Light Zone (seconds, 0-65535).</param>
    /// <param name="timeInModerateZone">Time in Moderate Zone (seconds, 0-65535).</param>
    /// <param name="timeInHardZone">Time in Hard Zone (seconds, 0-65535).</param>
    public Task SetTargetedTimeInThreeHeartRateZonesAsync(int timeInLightZone, int timeInModerateZone, int timeInHardZone)
        => WriteControlPointWithPayloadAsync(0x0F, [
            (byte)(timeInLightZone is >= 0 and <= 0xFFFF ? timeInLightZone : throw new ArgumentOutOfRangeException(nameof(timeInLightZone))),
            (byte)((timeInLightZone >> 8) & 0xFF),
            (byte)(timeInModerateZone is >= 0 and <= 0xFFFF ? timeInModerateZone : throw new ArgumentOutOfRangeException(nameof(timeInModerateZone))),
            (byte)((timeInModerateZone >> 8) & 0xFF),
            (byte)(timeInHardZone is >= 0 and <= 0xFFFF ? timeInHardZone : throw new ArgumentOutOfRangeException(nameof(timeInHardZone))),
            (byte)((timeInHardZone >> 8) & 0xFF)
        ]);

    /// <summary>
    /// Sets the targeted time in five heart rate zones (Very Light, Light, Moderate, Hard, Maximum) in seconds.
    /// </summary>
    /// <param name="timeInVeryLightZone">Time in Very Light Zone (seconds, 0-65535).</param>
    /// <param name="timeInLightZone">Time in Light Zone (seconds, 0-65535).</param>
    /// <param name="timeInModerateZone">Time in Moderate Zone (seconds, 0-65535).</param>
    /// <param name="timeInHardZone">Time in Hard Zone (seconds, 0-65535).</param>
    /// <param name="timeInMaximumZone">Time in Maximum Zone (seconds, 0-65535).</param>
    public Task SetTargetedTimeInFiveHeartRateZonesAsync(int timeInVeryLightZone, int timeInLightZone, int timeInModerateZone, int timeInHardZone, int timeInMaximumZone)
        => WriteControlPointWithPayloadAsync(0x10, [
            (byte)(timeInVeryLightZone is >= 0 and <= 0xFFFF ? timeInVeryLightZone : throw new ArgumentOutOfRangeException(nameof(timeInVeryLightZone))),
            (byte)((timeInVeryLightZone >> 8) & 0xFF),
            (byte)(timeInLightZone is >= 0 and <= 0xFFFF ? timeInLightZone : throw new ArgumentOutOfRangeException(nameof(timeInLightZone))),
            (byte)((timeInLightZone >> 8) & 0xFF),
            (byte)(timeInModerateZone is >= 0 and <= 0xFFFF ? timeInModerateZone : throw new ArgumentOutOfRangeException(nameof(timeInModerateZone))),
            (byte)((timeInModerateZone >> 8) & 0xFF),
            (byte)(timeInHardZone is >= 0 and <= 0xFFFF ? timeInHardZone : throw new ArgumentOutOfRangeException(nameof(timeInHardZone))),
            (byte)((timeInHardZone >> 8) & 0xFF),
            (byte)(timeInMaximumZone is >= 0 and <= 0xFFFF ? timeInMaximumZone : throw new ArgumentOutOfRangeException(nameof(timeInMaximumZone))),
            (byte)((timeInMaximumZone >> 8) & 0xFF)
        ]);

    /// <summary>
    /// Sets indoor bike simulation parameters.
    /// </summary>
    /// <param name="windSpeed">Wind speed (short, -32768 to 32767, 0.001 m/s units).</param>
    /// <param name="grade">Grade (short, -32768 to 32767, 0.01% units).</param>
    /// <param name="crr">Coefficient of rolling resistance (byte, 0-255, 0.00001 units).</param>
    /// <param name="cw">Wind resistance coefficient (byte, 0-255, 0.01 units).</param>
    public Task SetIndoorBikeSimulationParametersAsync(int windSpeed, int grade, int crr, int cw)
        => WriteControlPointWithPayloadAsync(0x11, [
            (byte)(windSpeed is >= short.MinValue and <= short.MaxValue ? windSpeed : throw new ArgumentOutOfRangeException(nameof(windSpeed))),
            (byte)((windSpeed >> 8) & 0xFF),
            (byte)(grade is >= short.MinValue and <= short.MaxValue ? grade : throw new ArgumentOutOfRangeException(nameof(grade))),
            (byte)((grade >> 8) & 0xFF),
            (byte)(crr is >= 0 and <= 0xFF ? crr : throw new ArgumentOutOfRangeException(nameof(crr))),
            (byte)(cw is >= 0 and <= 0xFF ? cw : throw new ArgumentOutOfRangeException(nameof(cw)))
        ]);

    /// <summary>
    /// Sets the wheel circumference in 0.1 mm units.
    /// </summary>
    /// <param name="mm">Wheel circumference (0-65535, 0.1 mm units).</param>
    public Task SetWheelCircumferenceAsync(int mm)
        => WriteControlPointWithPayloadAsync(0x12, BitConverter.GetBytes((ushort)(mm is >= 0 and <= 0xFFFF ? mm : throw new ArgumentOutOfRangeException(nameof(mm)))));

    /// <summary>
    /// Sends a spin down control command and returns the target speed low and high (in 0.01 km/h units) on success.
    /// </summary>
    /// <param name="controlType">Spin down control type (Start or Ignore).</param>
    public Task SpinDownControlAsync(SpinDownControlType controlType) => WriteControlPointAsync([0x13, (byte)controlType]);

    /// <summary>
    /// Sets the targeted cadence in 0.5 1/min units.
    /// </summary>
    /// <param name="cadence">Targeted cadence (0-65535, 0.5 1/min units).</param>
    public Task SetTargetedCadenceAsync(int cadence)
        => WriteControlPointWithPayloadAsync(0x14, BitConverter.GetBytes((ushort)(cadence is >= 0 and <= 0xFFFF ? cadence : throw new ArgumentOutOfRangeException(nameof(cadence)))));

    private Task WriteControlPointWithPayloadAsync(byte opCode, byte[] payload)
    {
        var fullPayload = new byte[1 + payload.Length];
        fullPayload[0] = opCode;
        payload.CopyTo(fullPayload, 1);
        return WriteControlPointAsync(fullPayload);
    }

    private async Task WriteControlPointAsync(byte opCode) => await WriteControlPointAsync([opCode]);

    private async Task WriteControlPointAsync(byte[] payload)
    {
        var characteristic = _characteristics.FirstOrDefault(c => c.Uuid == FitnessDeviceGuids.FitnessMachineControlPointCharacteristicUuid)
            ?? throw new InvalidOperationException("FTMS Control Point characteristic not found.");

        var writer = new DataWriter { ByteOrder = ByteOrder.LittleEndian };
        writer.WriteBytes(payload);
        var buffer = writer.DetachBuffer();
        var status = await characteristic.WriteValueAsync(
            buffer,
            GattWriteOption.WriteWithResponse);
        if (status != GattCommunicationStatus.Success)
            throw new InvalidOperationException($"Failed to write to FTMS Control Point: {status}");
    }
}
