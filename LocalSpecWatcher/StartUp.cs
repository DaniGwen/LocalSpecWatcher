using System;

namespace LocalSpecWatcher
{
    public class StartUp
    {
        static void Main()
        {
            var systemInfo = new SystemInfo();

            Console.WriteLine("OS: " + systemInfo.OperatingSystem);
            Console.WriteLine();
            Console.WriteLine("Hostname: " + systemInfo.Hostname);
            Console.WriteLine();
            Console.WriteLine("Ram: " + systemInfo.Ram);
            Console.WriteLine();
            Console.WriteLine("Processor: " +  systemInfo.Processor);            
            Console.WriteLine();
            Console.WriteLine("Drives: " + systemInfo.Drives);
        }
    }
}
