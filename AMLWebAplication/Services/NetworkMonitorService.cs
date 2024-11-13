using System.Net.NetworkInformation;

namespace AMLWebAplication.Services
{
    public class NetworkMonitorService
    {
        public long BytesSent { get; private set; }
        public long BytesReceived { get; private set; }

        public void UpdateNetworkTraffic()
        {
            foreach (var interfaceStat in NetworkInterface.GetAllNetworkInterfaces())
            {
                var stats = interfaceStat.GetIPv4Statistics();
                BytesSent += stats.BytesSent;
                BytesReceived += stats.BytesReceived;
            }
        }
    }
}
