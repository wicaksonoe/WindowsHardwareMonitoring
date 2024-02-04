using System.Text.RegularExpressions;

namespace HardwareMonitor.Core.Objects;

public class TargetSensor
{
    public HardwareType HardwareType { get; }
    public Regex SensorName { get; }
    public SensorType SensorType { get; }

    public TargetSensor(HardwareType hwType, string sensorName, SensorType sensorType)
    {
        HardwareType = hwType;
        SensorName = new Regex($"^{sensorName}");
        SensorType = sensorType;
    }
        
    public TargetSensor(HardwareType hwType, Regex regexSensorName, SensorType sensorType)
    {
        HardwareType = hwType;
        SensorName = regexSensorName;
        SensorType = sensorType;
    }
}