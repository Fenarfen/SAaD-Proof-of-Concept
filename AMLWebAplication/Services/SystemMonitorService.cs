using System.Diagnostics;

namespace AMLWebAplication.Services
{
    public class SystemMonitorService
    {
        private float cpuUsage;
        private float availableMemory;
        private float diskUsage;
        
        public event Action<string> CriticalAlert;

        public async Task<SystemMonitorData> GetSystemDataAsync()
        {
            await UpdateSystemStats();
            CheckCriticalConditions();
            
            return new SystemMonitorData
            {
                CpuUsage = cpuUsage,
                MemoryUsage = availableMemory,
                DiskUsage = diskUsage
            };
        }

        private async Task UpdateSystemStats()
        {
            using var process = Process.GetCurrentProcess();
            availableMemory = process.WorkingSet64 / (1024f * 1024f); // Convert to MB
            cpuUsage = await CalculateCpuUsage(process);
            diskUsage = CalculateDiskUsage();
        }

        private void CheckCriticalConditions()
        {
            if (cpuUsage > 85)
            {
                CriticalAlert?.Invoke($"High CPU usage detected: {cpuUsage:F1}%");
            }
            
            if (availableMemory > 500) // More than 500 MB being used
            {
                CriticalAlert?.Invoke($"High memory usage detected: {availableMemory:F1} MB");
            }
            
            if (diskUsage > 95)
            {
                CriticalAlert?.Invoke($"Critical disk usage: {diskUsage:F1}%");
            }
        }

        private async Task<float> CalculateCpuUsage(Process process)
        {
            var startCpuUsage = process.TotalProcessorTime;
            var startTime = DateTime.UtcNow;
            
            await Task.Delay(500); // Wait for 500ms to get a meaningful measurement
            
            var endCpuUsage = process.TotalProcessorTime;
            var endTime = DateTime.UtcNow;
            
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            
            return (float)(cpuUsageTotal * 100);
        }

        private float CalculateDiskUsage()
        {
            var rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            if (rootDirectory != null)
            {
                var driveInfo = new DriveInfo(rootDirectory);
                return (float)(driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / driveInfo.TotalSize * 100;
            }
            return 0;
        }
    }

    public class SystemMonitorData
    {
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
        public float DiskUsage { get; set; }
    }
}