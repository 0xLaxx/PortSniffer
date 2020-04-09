using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Client
{
    public class SampleData
    {
        public string Username { get; set; }
        public List<Harddrive> Harddrives { get; set; }
        public string MachineName { get; set; }
        public string OSVersion { get; set; }
        public bool Is64Bit { get; set; }
        public string GPU { get; set; }
        public string CPU { get; set; }

        //public string Mainboard { get; set; }
        //public string RAM { get; set; }
        //public string WindowsVersion { get; set; }
        //public string PatchLevel { get; set; }
        //public string NetFramework { get; set; }

        public SampleData()
        {
            Username = Environment.UserName;

            Harddrives = DriveInfo.GetDrives().Select(d => new Harddrive
            {
                Name = d.Name,
                FreeSpaceInBytes = d.TotalFreeSpace.ToString(),
                TotalSize = d.TotalSize.ToString()
            }).ToList();

            MachineName = Environment.MachineName;
            OSVersion = Environment.OSVersion.VersionString;
            Is64Bit = Environment.Is64BitOperatingSystem;
            GPU = GetGPUInfo();
            CPU = GetCPUInfo();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        string GetGPUInfo()
        {
            ManagementObjectSearcher gpus = new ManagementObjectSearcher(
                new ObjectQuery("select * from Win32_VideoController"));

            string gpuname = string.Empty;
            foreach (ManagementObject gpu in gpus.Get())
            {
                gpuname = gpu["name"].ToString();
            }

            return gpuname;
        }

        string GetCPUInfo()
        {
            ManagementObjectSearcher cpus = new ManagementObjectSearcher(
                new ObjectQuery("select * from Win32_Processor"));

            string cpuName = string.Empty;
            foreach (ManagementObject cpu in cpus.Get())
            {
                cpuName = cpu["name"].ToString();
            }

            return cpuName.Trim();
        }
    }

    public class Harddrive
    {
        public string Name { get; set; }
        public string FreeSpaceInBytes { get; set; }
        public string TotalSize { get; set; }
    }
}
