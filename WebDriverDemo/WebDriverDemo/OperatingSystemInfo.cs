using System;
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;   //This namespace is used to work with WMI classes. For using this namespace add reference of System.Management.dll .
using Microsoft.Win32;     //This namespace is used to work with Registry editor.

namespace WebDriverDemo
{
   
    public class SystemInfo
    {
        public void getOperatingSystemInfo()
        {
            double div = 1000000000;
            ComputerInfo myCompInfo = new ComputerInfo();
            //Console.WriteLine("Checking operating system info....\n");

            Program.LogResults("Total Physical Memory= " + (myCompInfo.TotalPhysicalMemory/div).ToString("F") + "GB");
            Program.LogResults("Total Virtual Memory= " + (myCompInfo.TotalVirtualMemory / div).ToString("F") + "GB");
            Program.LogResults("Available Physical Memory= " + (myCompInfo.AvailablePhysicalMemory / div).ToString("F") + "GB");
            Program.LogResults("Available Virtual Memory= " + (myCompInfo.AvailableVirtualMemory / div).ToString("F") + "GB");
            Program.LogResults("Installed UI Culture= " + myCompInfo.InstalledUICulture);
            Program.LogResults("Opetating System= " + myCompInfo.OSFullName);
            

            
            //Create an object of ManagementObjectSearcher class and pass query as parameter.
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get())
            {
                
                if (managementObject["CSDVersion"] != null)
                {

                        //Display operating system version.
                    Program.LogResults("Operating System Service Pack   :  " + managementObject["CSDVersion"].ToString());
                }


            }

        }



        public void getProcessorInfo()
        {
            Program.LogResults("\n\nProcessor Name: ");
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.

            if (processor_name != null)
            {
                if (processor_name.GetValue("ProcessorNameString") != null)
                {
                    //Console.WriteLine(processor_name.GetValue("ProcessorNameString"));   //Display processor info.
                    Program.LogResults(processor_name.GetValue("ProcessorNameString").ToString());
                }
            }
        }
    }
}