namespace HardwareMonitor.Core.Objects;

public class SensorData(string hardwareName, string sensorName, SensorType sensorType, float? value)
{
    public string HardwareName { get; } = hardwareName;
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
        _ => "",
    };
}