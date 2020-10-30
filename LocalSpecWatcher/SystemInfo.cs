using System;
using System.IO;
using System.Management;
using System.Text;
using System.Timers;
using Topshelf;

namespace LocalSpecWatcher
{
    public class SystemInfo : ServiceControl
    {
        public string Hostname { get; private set; }

        public string OperatingSystem { get; private set; }

        public string Processor { get; private set; }

        public string Ram { get; private set; }

        public string Drives { get; private set; }

        private readonly string AmcUrl = "https://lwlsv926/lwl/api/Amc/";

        private static Timer timer;

        public SystemInfo()
        {
            timer = new Timer();
            // 4hrs = 14400000ms
            timer.Interval = 10000;  //For test every 10 seconds gets system data
            timer.Elapsed += OnTimeUpEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public bool Start(HostControl hostControl)
        {
            //Initialize client
            //Save info in SystemInfoModel

            this.CollectSystemInfo();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }

        //Event handler
        private void OnTimeUpEvent(object sender, ElapsedEventArgs e)
        {
            this.CollectSystemInfo();

            Console.WriteLine("Properties updated!");
        }

        private void CollectSystemInfo()
        {
            this.OperatingSystem = this.GetOperatingSystem();
            this.Ram = this.GetRam();
            this.Processor = this.GetProcessor();
            this.Hostname = GetHostname();
            this.Drives = GetDrivesInfo();

            Console.WriteLine("System info Updated");
        }

        private string GetProcessor()
        {
            string processor = "Win32_Processor";

            ManagementObjectSearcher processorObj = new ManagementObjectSearcher("select * from " + processor);

            string processorClock = string.Empty;
            string processorCores = string.Empty;
            string result = string.Empty;
            string measuringUnit = string.Empty;

            foreach (ManagementObject obj in processorObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    if (property.Name.Contains("NumberOfLogicalProcessors"))
                    {
                        processorCores = property.Value.ToString();
                    }
                    else if (property.Name.Contains("MaxClockSpeed"))
                    {
                        int value = int.Parse(property.Value.ToString());

                        measuringUnit = value > 1000 ? "Ghz" : "Mhz";

                        processorClock = value.ToString();
                    }
                }
            }
            result = $"{processorCores} virtual cores / {processorClock} {measuringUnit}";

            return result;
        }

        private string GetRam()
        {
            string physicalMemory = "Win32_PhysicalMemory";

            ManagementObjectSearcher physicalMemoryObj = new ManagementObjectSearcher("select * from " + physicalMemory);

            long ramSize = 0;
            string ramSizeString = string.Empty;
            string result = string.Empty;
            string measuringUnit = string.Empty;

            foreach (ManagementObject obj in physicalMemoryObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName.StartsWith("Capacity"))
                    {
                        ramSize += long.Parse(property.Value.ToString());
                    }
                }
            }
            result = this.SizeSuffix(ramSize, 0);

            return result;
        }

        private string GetDrivesInfo()
        {
            var stringBuilder = new StringBuilder();

            var drives = DriveInfo.GetDrives();

            foreach (DriveInfo d in drives)
            {
                var freeSpace = this.SizeSuffix(d.AvailableFreeSpace);
                var totalSize = this.SizeSuffix(d.TotalSize);

                stringBuilder.AppendLine($"{d.Name} {freeSpace} free of {totalSize}");
            }

            return stringBuilder.ToString();
        }

        private string GetOperatingSystem()
        {
            string host = "Win32_OperatingSystem";
            string os = string.Empty;

            ManagementObjectSearcher osObj = new ManagementObjectSearcher("select * from " + host);

            foreach (ManagementObject obj in osObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    if (property.Name.StartsWith("Caption"))
                    {
                        os = property.Value.ToString();
                    }
                }
            }

            return os;
        }

        private string GetHostname()
        {
            var hostname = Environment.MachineName;

            return hostname;
        }

        //Convert bytes and add suffix
        private string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            string[] SizeSuffixes =
                    { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
