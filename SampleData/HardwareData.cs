using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SampleData
{
    public class HardwareData
    {
        #region Properties
        public string Username { get; set; }
        public int Harddrives { get; set; }
        public string FreeSpaceInGB { get; set; }
        public bool Is64Bit { get; set; }
        public string GPU { get; set; }
        public string CPU { get; set; }
        public string Mainboard { get; set; }
        public string RAM { get; set; }
        public string WindowsVersion { get; set; }
        public string PatchLevel { get; set; }
        public string NetFramework { get; set; }
        #endregion
        
        #region Fields
        [JsonIgnore]
        Random random = new Random();

        static string firstNamesTxt = Properties.Resources.firstnames;
        static string lastNamesTxt = Properties.Resources.lastnames;
        static string gpuTxt = Properties.Resources.gpu;
        static string cpuTxt = Properties.Resources.cpu;
        static string mainboardTxt = Properties.Resources.mainboard;
        static string ramTxt = Properties.Resources.ram;
        static string windowsVersionsTxt = Properties.Resources.windows;
        static string netFrameworkTxt = Properties.Resources.netframework;
        static string patchlevelTxt = Properties.Resources.patchlevel;
        #endregion

        //generates random sample data and randomly populates properties
        public HardwareData()
        {
            if (random.Next() % 2 == 0)
                Username = $"{GenerateRandomData(firstNamesTxt)} {GenerateRandomData(lastNamesTxt)}";

            if (random.Next() % 2 == 0)
                Harddrives = random.Next(4);

            if (random.Next() % 2 == 0)
                FreeSpaceInGB = random.Next(2000).ToString();

            if (random.Next() % 2 == 0)
                Is64Bit = random.Next() % 2 == 0 ? true : false;

            if (random.Next() % 2 == 0)
                GPU = GenerateRandomData(gpuTxt);

            if (random.Next() % 2 == 0)
                CPU = GenerateRandomData(cpuTxt);

            if (random.Next() % 2 == 0)
                Mainboard = GenerateRandomData(mainboardTxt);

            if (random.Next() % 2 == 0)
                RAM = GenerateRandomData(ramTxt);

            if (random.Next() % 2 == 0)
                WindowsVersion = GenerateRandomData(windowsVersionsTxt);

            if (random.Next() % 2 == 0)
                PatchLevel = GenerateRandomData(patchlevelTxt);

            if (random.Next() % 2 == 0)
                NetFramework = GenerateRandomData(netFrameworkTxt);
        }

        //reads the textfiles and gives back a random string
        private string GenerateRandomData(string textfile)
        {
            List<string> results;

            try
            {
                textfile = textfile.Replace("\r", "");
                results = new List<string>(textfile.Split('\n'));

                return results[random.Next(results.Count)];
            }
            catch
            {
                return string.Empty;
            }

        }

        //serializes as json and ignores the properties that did not get populated in constructor
        public string GenerateJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }
    }
}
