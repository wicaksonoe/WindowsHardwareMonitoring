using System.Text.RegularExpressions;
using HardwareMonitor.Core.Constants;
using HardwareMonitor.Core.Objects;

namespace HardwareMonitor.Core;

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}

public static class Main
{
    public static IEnumerable<SensorData> Monitor()
    {
        var computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
        };

        computer.Open();
        computer.Accept(new UpdateVisitor());

        var targetHardwareTypes = new[] { HardwareType.Cpu, HardwareType.Memory, HardwareType.GpuNvidia };
        var hardwareList = computer.Hardware.Where(h => targetHardwareTypes.Contains(h.HardwareType));

        var monitoredSensors = new List<TargetSensor>
        {
            new (HardwareType.Cpu, new Regex(@"^CPU Core #\d\b"), SensorType.Clock),
            new (HardwareType.Cpu, SensorName.CpuTotal, SensorType.Load),
            new (HardwareType.Cpu, SensorName.CpuCoreAverage, SensorType.Temperature),
            new (HardwareType.Memory, SensorName.MemoryAvailable, SensorType.Data),
            new (HardwareType.Memory, SensorName.MemoryUsed, SensorType.Data),
            new (HardwareType.GpuNvidia, SensorName.GpuCore, SensorType.Clock),
            new (HardwareType.GpuNvidia, SensorName.GpuMemory, SensorType.Clock),
            new (HardwareType.GpuNvidia, SensorName.GpuFan, SensorType.Control),
            new (HardwareType.GpuNvidia, SensorName.GpuPackage, SensorType.Power),
            new (HardwareType.GpuNvidia, SensorName.GpuHotSpot, SensorType.Temperature),
            new (HardwareType.GpuNvidia, SensorName.GpuMemoryTotal, SensorType.SmallData),
            new (HardwareType.GpuNvidia, SensorName.GpuMemoryUsed, SensorType.SmallData),
        };

        var data = new List<SensorData>();
        foreach (var hardware in hardwareList)
        {
            var sensors = hardware.Sensors
                .Where(s => monitoredSensors
                    .Where(m => m.HardwareType == s.Hardware.HardwareType)
                    .Any(m => m.SensorName.IsMatch(s.Name) && m.SensorType == s.SensorType)
                )
                .OrderBy(s => s.SensorType)
                .ToArray();

            float cpuCoreAggregate = 0;
            var cpuCount = 0;
            
            float totalMemory = 0;
            
            foreach (var sensor in sensors)
            {
                switch (sensor.Hardware.HardwareType)
                {
                    case HardwareType.Cpu when sensor.SensorType == SensorType.Clock:
                        cpuCoreAggregate += sensor.Value ?? 0;
                        cpuCount += 1;
                        break;
                    
                    case HardwareType.Memory:
                    {
                        totalMemory += sensor.Value ?? 0;

                        if (sensor.Name == SensorName.MemoryAvailable)
                        {
                            data.Add(new SensorData(hardware.Name, hardware.HardwareType, SensorName.MemoryTotal, sensor.SensorType, totalMemory));
                            break;
                        }

                        data.Add(new SensorData(hardware.Name, hardware.HardwareType, sensor.Name, sensor.SensorType, sensor.Value));
                        break;
                    }
                    
                    default:
                        data.Add(new SensorData(hardware.Name, hardware.HardwareType, sensor.Name, sensor.SensorType, sensor.Value));
                        break;
                }
            }

            if (cpuCount > 0)
            {
                data.Add(new SensorData(hardware.Name, hardware.HardwareType, SensorName.CpuClockAverage, SensorType.Clock, cpuCoreAggregate / cpuCount));
            }
        }

        computer.Close();

        return data;
    }
}