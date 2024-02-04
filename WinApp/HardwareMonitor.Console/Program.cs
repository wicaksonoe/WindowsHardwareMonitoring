using HardwareMonitor.Core;
using HardwareMonitor.Core.Constants;
using LibreHardwareMonitor.Hardware;

do
{
    var data = Main.Monitor().ToList();
    var groups = data.GroupBy(d => d.HardwareType).ToList();

    Console.Clear();
    foreach (var group in groups)
    {
        var items = group.ToList();
    
        switch (group.Key)
        {
            case HardwareType.Cpu:
            {
                var cpuTemp = items.FirstOrDefault(i => i.SensorName == SensorName.CpuCoreAverage);
                var cpuUsage = items.FirstOrDefault(i => i.SensorName == SensorName.CpuTotal);
                var cpuClock = items.FirstOrDefault(i => i.SensorName == SensorName.CpuClockAverage);

                Console.WriteLine("CPU Clock\t\t= {0:n1} {1} | {2:n1} {3} | {4:n1} {5}", cpuClock?.Value, cpuClock?.Unit, cpuUsage?.Value, cpuUsage?.Unit, cpuTemp?.Value, cpuTemp?.Unit);
                Console.WriteLine("CPU Temperature\t\t= {0:n1} {1}\n", cpuTemp?.Value, cpuTemp?.Unit);
                break;
            }
            
            case HardwareType.Memory:
            {
                var memoryUsage = items.FirstOrDefault(i => i.SensorName == SensorName.MemoryUsed);
                var memoryTotal = items.FirstOrDefault(i => i.SensorName == SensorName.MemoryTotal);

                Console.WriteLine("RAM\t\t\t= {0:n1} {1} / {2:n1} {3} | {4:n1} %\n", memoryUsage?.Value, memoryUsage?.Unit, memoryTotal?.Value, memoryTotal?.Unit, memoryUsage?.Value / memoryTotal?.Value * 100);
                break;
            }
            
            case HardwareType.GpuNvidia:
            {
                var gpuPower = items.FirstOrDefault(i => i.SensorName == SensorName.GpuPackage);
                var gpuCoreClock = items.FirstOrDefault(i => i.SensorName == SensorName.GpuCore);
                var gpuMemoryClock = items.FirstOrDefault(i => i.SensorName == SensorName.GpuMemory);
                var gpuHotSpotTemp = items.FirstOrDefault(i => i.SensorName == SensorName.GpuHotSpot);
                var gpuFanUtil = items.FirstOrDefault(i => i.SensorName == SensorName.GpuFan);
                var gpuMemoryUsed = items.FirstOrDefault(i => i.SensorName == SensorName.GpuMemoryUsed);
                var gpuMemoryTotal = items.FirstOrDefault(i => i.SensorName == SensorName.GpuMemoryTotal);

                Console.WriteLine("GPU Core Clock\t\t= {0:n1} {1}", gpuCoreClock?.Value, gpuCoreClock?.Unit);
                Console.WriteLine("Video Memory\t\t= {0:n1} {1} | {2:n1} {3} / {4:n1} {5} | {6:n1} %", gpuMemoryClock?.Value, gpuMemoryClock?.Unit, gpuMemoryUsed?.Value, gpuMemoryUsed?.Unit, gpuMemoryTotal?.Value, gpuMemoryTotal?.Unit, gpuMemoryUsed?.Value / gpuMemoryTotal?.Value * 100);
                Console.WriteLine("Power\t\t\t= {0:n1} {1} | {2:n1} {3}", gpuPower?.Value, gpuPower?.Unit, gpuHotSpotTemp?.Value, gpuHotSpotTemp?.Unit);
                Console.WriteLine("Fan Util\t\t= {0:n1} {1}", gpuFanUtil?.Value, gpuFanUtil?.Unit);
                break;
            }
        }
    }
    
    Thread.Sleep(1000);
} while (true);
    