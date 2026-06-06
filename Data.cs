using System.Diagnostics;
using System.Runtime.Versioning;
using System.Management;

[SupportedOSPlatform("windows")]

public class MyDataManager
{
    // CPU counter 
    private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    // RAM Process counter
    private PerformanceCounter availableGB = new PerformanceCounter("Memory", "Available MBytes");
    // Function to get the total amount of RAM
    static double GetTotalRam()
    {
        // query to get a "TotalPhysicalMemory" from "Win32_ComputerSystem" object
        string query = "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem";
        /*
            ManagementObjectSearcher(string queryString);

        */
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

        // for loop that loops through searcher.Get()
        foreach (ManagementObject obj in searcher.Get())
        {
            // from the loop gets "TotalPhysicalMemory"
            ulong bytes = (ulong)obj["TotalPhysicalMemory"];
            // return bytes as gigabytes.
            return bytes / 1024.0 / 1024.0 / 1024.0;
        }
        return 0;
    }
    // Get total ram
    double TotalRam = GetTotalRam();

    public (double Cpu, double UsedRamPercent, double FreeRam, List<(string Name, double Ram)> Procs) GetCurrentSystemSnapShot()
    {
        double cpuUsage = cpuCounter.NextValue();
        double availableRam = availableGB.NextValue() / 1024.0;
        double ramUsagePercent = ((TotalRam - availableRam) / TotalRam) * 100;

        List<(string Name, double Ram)> procs = new List<(string, double)>();
        Process[] processList = Process.GetProcesses();

        foreach (Process p in processList)
        {
            try
            {
                double ramUsageMB = p.WorkingSet64 / 1024.0 / 1024.0;
                if (ramUsageMB > 100.0)
                {
                    procs.Add((p.ProcessName, ramUsageMB));
                }
            }
            catch
            {
                continue;
            }
        }

        var sortedProces = procs.OrderByDescending(x => x.Ram).ToList();
        return (cpuUsage, ramUsagePercent, availableRam, sortedProces);
    }
}