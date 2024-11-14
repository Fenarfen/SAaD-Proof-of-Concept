using System.Net.NetworkInformation;

namespace AMLWebAplication.Services
{
    public class NetworkMonitorService
    {
        private long _previousBytesSent;
        private long _previousBytesReceived;
        
        public long BytesSent { get; private set; }
        public long BytesReceived { get; private set; }
        public event Action<string> CriticalAlert;

        public void UpdateNetworkTraffic()
        {
            long totalBytesSent = 0;
            long totalBytesReceived = 0;

            foreach (var interfaceStat in NetworkInterface.GetAllNetworkInterfaces())
            {
                var stats = interfaceStat.GetIPv4Statistics();
                totalBytesSent += stats.BytesSent;
                totalBytesReceived += stats.BytesReceived;
            }

            long sentIncrease = totalBytesSent - _previousBytesSent;
            long receivedIncrease = totalBytesReceived - _previousBytesReceived;

            BytesSent = totalBytesSent;
            BytesReceived = totalBytesReceived;
            _previousBytesSent = totalBytesSent;
            _previousBytesReceived = totalBytesReceived;

            CheckCriticalNetworkConditions(sentIncrease, receivedIncrease);
        }

        private void CheckCriticalNetworkConditions(long sentIncrease, long receivedIncrease)
        {
            const long criticalTrafficThreshold = 1_000_000_000; // 1 GB
            
            if (sentIncrease > criticalTrafficThreshold)
            {
                CriticalAlert?.Invoke($"Critical Alert: Unusually high network upload detected: {sentIncrease / 1_000_000} MB");
            }
            
            if (receivedIncrease > criticalTrafficThreshold)
            {
                CriticalAlert?.Invoke($"Critical Alert: Unusually high network download detected: {receivedIncrease / 1_000_000} MB");
            }
        }
    }
}