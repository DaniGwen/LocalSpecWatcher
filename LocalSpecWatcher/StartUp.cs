using System;
using System.Management;
using System.Linq;

namespace LocalSpecWatcher
{
    public class StartUp
    {
        static void Main()
        {
            GetProcessorInfo();
            GetPartitionInfo();
            GetDynamicMemoryInfo();
            GetBatteryInfo();
            GetGpuInfo();
        }

        private static void GetProcessorInfo()
        {
            string processor = "Win32_Processor";

            ManagementObjectSearcher processorObj = new ManagementObjectSearcher("select * from " + processor);

            Console.WriteLine("==========");
            Console.WriteLine("Processor:");
            Console.WriteLine("==========");

            foreach (ManagementObject obj in processorObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName.StartsWith("Name") ||
                        propertyName.Contains("NumberOfCores") ||
                        propertyName.Contains("CurrentClockSpeed") ||
                        propertyName.Contains("MaxClockSpeed"))
                    {
                        Console.WriteLine($"{property.Name}:  {property.Value}");
                    }
                }
            }
            Console.WriteLine();
        }

        private static void GetDynamicMemoryInfo()
        {
            string physicalMemory = "Win32_PhysicalMemory";

            ManagementObjectSearcher physicalMemoryObj = new ManagementObjectSearcher("select * from " + physicalMemory);

            Console.WriteLine("================");
            Console.WriteLine("Physical Memory:");
            Console.WriteLine("================");

            foreach (ManagementObject obj in physicalMemoryObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName.Contains("BankLabel") ||
                        propertyName.StartsWith("Capacity") ||
                        propertyName.StartsWith("Speed"))
                    {
                        Console.WriteLine($"{property.Name}: {property.Value}");
                    }
                }
            }
            Console.WriteLine();
        }

        private static void GetPartitionInfo()
        {
            string diskPartition = "Win32_DiskDrive";

            ManagementObjectSearcher partitionObj = new ManagementObjectSearcher("select * from " + diskPartition);

            Console.WriteLine("============");
            Console.WriteLine("Disk drives:");
            Console.WriteLine("============");

            foreach (ManagementObject obj in partitionObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName.StartsWith("Model") ||
                        propertyName.StartsWith("Size") ||
                        propertyName.StartsWith("SerialNumber") ||
                        propertyName.StartsWith("Partitions"))
                    {
                        Console.WriteLine($"{property.Name}:  {property.Value}");
                    }
                }
            }
            Console.WriteLine();
        }

        private static void GetBatteryInfo()
        {
            string battery = "Win32_Battery";

            ManagementObjectSearcher batteryObj = new ManagementObjectSearcher("select * from " + battery);

            Console.WriteLine("========");
            Console.WriteLine("Battery:");
            Console.WriteLine("========");

            foreach (ManagementObject obj in batteryObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName == "StatusInfo")
                    {
                        continue;
                    }

                    if (propertyName.StartsWith("Description") ||
                        propertyName.StartsWith("Name") ||
                        propertyName.Contains("EstimatedChargeRemaining") ||
                        propertyName.Contains("EstimatedRunTime") ||
                        propertyName.StartsWith("Status"))
                    {
                        Console.WriteLine(propertyName == "Name" ? $"Model: {property.Value}" : $"{property.Name}: {property.Value}");
                    }
                }
            }
            Console.WriteLine();
        }

        private static void GetGpuInfo()
        {
            string gpu = "Win32_DisplayControllerConfiguration";

            ManagementObjectSearcher gpuObj = new ManagementObjectSearcher("select * from " + gpu);

            Console.WriteLine("==============");
            Console.WriteLine("Video adapter:");
            Console.WriteLine("==============");
                         
            foreach (ManagementObject obj in gpuObj.Get())
            {
                foreach (PropertyData property in obj.Properties)
                {
                    var propertyName = property.Name;

                    if (propertyName.StartsWith("Description") ||
                        propertyName.StartsWith("VideoMode"))
                    {
                        Console.WriteLine($"{property.Name}:  {property.Value}");
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
