using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AMLWebAplication.Services
{
    public class SystemMonitorService
    {
        private float cpuUsage;
        private float availableMemory;
        private float diskUsage;

        public async Task<SystemMonitorData> GetSystemDataAsync()
        {
            UpdateSystemStats();
            return new SystemMonitorData
            {
                CpuUsage = cpuUsage,
                MemoryUsage = availableMemory,
                DiskUsage = diskUsage
            };
        }

        private void UpdateSystemStats()
        {
            using var process = Process.GetCurrentProcess();
            availableMemory = (float)process.WorkingSet64 / (1024 * 1024);
            cpuUsage = CalculateCpuUsage(process);
            diskUsage = CalculateDiskUsage();
        }

        private float CalculateCpuUsage(Process process)
        {
            var startCpuUsage = process.TotalProcessorTime;
            var startTime = DateTime.UtcNow;
            Task.Delay(500).Wait();
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
            var driveInfo = new DriveInfo(rootDirectory);
            return (float)(driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / driveInfo.TotalSize * 100;
        }
    }

    public class SystemMonitorData
    {
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
        public float DiskUsage { get; set; }
    }
}