using System.Text.RegularExpressions;
using HardwareMonitor.App.Objects;

namespace HardwareMonitor.App;

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
    public static void Monitor()
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
            new (HardwareType.Cpu, "CPU Total", SensorType.Load),
            new (HardwareType.Cpu, "Core Average", SensorType.Temperature),
            new (HardwareType.Memory, "Memory Available", SensorType.Data),
            new (HardwareType.Memory, "Memory Used", SensorType.Data),
            new (HardwareType.GpuNvidia, "GPU Core", SensorType.Clock),
            new (HardwareType.GpuNvidia, "GPU Core", SensorType.Load),
            new (HardwareType.GpuNvidia, "GPU Memory", SensorType.Clock),
            new (HardwareType.GpuNvidia, "GPU Fan", SensorType.Control),
            new (HardwareType.GpuNvidia, "GPU Board Power", SensorType.Load),
            new (HardwareType.GpuNvidia, "GPU Hot Spot", SensorType.Temperature),
            new (HardwareType.GpuNvidia, "GPU Memory Total", SensorType.SmallData),
            new (HardwareType.GpuNvidia, "GPU Memory Used", SensorType.SmallData),
        };

        var data = new List<SensorData>();
        foreach (var hardware in hardwareList)
        {
            // Console.WriteLine("Hardware: {0}", hardware.Name);

            var sensors = hardware.Sensors
                .Where(s => monitoredSensors
                    .Where(m => m.HardwareType == s.Hardware.HardwareType)
                    .Any(m => m.SensorName.IsMatch(s.Name) && m.SensorType == s.SensorType)
                )
                .OrderBy(s => s.SensorType)
                .ToArray();

            float cpuCoreAggregate = 0;
            var cpuCount = 0;
            
            foreach (var sensor in sensors)
            {
                // Console.WriteLine("\tSensor: {0}, sensorType: {1}, value: {2}", sensor.Name, sensor.SensorType, sensor.Value);
                if (sensor.Hardware.HardwareType == HardwareType.Cpu && sensor.SensorType == SensorType.Clock)
                {
                    cpuCoreAggregate += sensor.Value ?? 0;
                    cpuCount += 1;
                    continue;
                }
                
                data.Add(new SensorData(hardware.Name, sensor.Name, sensor.SensorType, sensor.Value));
            }

            if (cpuCount > 0)
            {
                data.Add(new SensorData(hardware.Name, $"CPU Core Average ({cpuCount} Core)", SensorType.Clock, cpuCoreAggregate / cpuCount));
            }
        }

        foreach (var (item, i) in data.Select((val, i) => new Tuple<SensorData, int>(val, i)))
        {
            // Console.WriteLine("Hardware: {3}, Sensor: {0}, sensorType: {1}, value: {2}", item.SensorName, item.SensorType, item.Value, item.HardwareName);
            var nextIndex = i + 1;
            if (nextIndex < data.Count && data[nextIndex].HardwareName != item.HardwareName)
            {
                Console.WriteLine("{0}, {1} = {2} {3}\n", item.HardwareName, item.SensorName, item.Value, item.Unit);
            }
            else
            {
                Console.WriteLine("{0}, {1} = {2} {3}", item.HardwareName, item.SensorName, item.Value, item.Unit);
            }
        }

        computer.Close();
    }
}