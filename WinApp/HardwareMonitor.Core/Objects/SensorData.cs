﻿namespace HardwareMonitor.Core.Objects;

public class SensorData(string hardwareName, HardwareType hardwareType, string sensorName, SensorType sensorType, float? value)
{
    public string HardwareName { get; } = hardwareName;
    public HardwareType HardwareType = hardwareType;
    public string SensorName { get; } = sensorName;
    public SensorType SensorType { get; } = sensorType;
    public float? Value { get; } = value;
    public string Unit { get; } = sensorType switch
    {
        SensorType.Clock => "Hz",
        SensorType.Temperature => "\u00b0C",
        SensorType.Data => "GB",
        SensorType.SmallData => "MB",
        SensorType.Control => "%",
        SensorType.Load => "%",
        SensorType.Power => "Watt",
        _ => "",
    };
}