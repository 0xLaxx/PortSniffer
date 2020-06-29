using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace SettingsLibrary
{
    public static class Settings
    {
        static Dictionary<string, object> SettingsDict { get; set; }
        public static T Get<T>(string key)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "server.userconfig.txt");
            SettingsProperties settings = new SettingsProperties();

            //load or initialize settings
            if (File.Exists(path)) //read settings
            {
                settings = Deserialize(path);
            }
            else //create settings for the first time
            {
                SerializeToXML(settings, path);
            }

            //initializes dictionary with settings
            SettingsDict = LoadDictionary(settings);
            try
            {
                //returns object of type T for key (settingsproperty)
                return (T)SettingsDict[key.ToLower()];
            }
            catch
            {
                //returns default value for type T if error
                return default;
            }
        }

        #region XML
        private static void SerializeToXML(SettingsProperties settings, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsProperties));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, settings);
            }
        }
        private static SettingsProperties Deserialize(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsProperties));

            SettingsProperties output;
            using (TextReader stream = new StreamReader(filename))
            {
                output = (SettingsProperties)serializer.Deserialize(stream);
            }

            return output;
        }

        #endregion

        #region Helper methods
        private static Dictionary<string, object> LoadDictionary(SettingsProperties settings)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();

            //gets properties with reflection and maps them to a dictionary
            foreach (var prop in settings.GetType().GetProperties())
            {
                output[prop.Name.ToLower()] = prop.GetValue(settings);
            }

            return output;
        }

        #endregion
    }

    //placeholder class for each setting
    public class SettingsProperties
    {
        public bool SaveToDatabase { get; set; } = true;
        public string Table { get; set; } = "SniffedData";
        public string Database { get; set; } = "PortlistenerDb";
        public string Server { get; set; } = "localhost\\SQLEXPRESS";
        public int Port { get; set; } = 55779;
        public string LogPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "port_listener.log.txt");
        public bool EnableLogger { get; set; } = true;

        [XmlIgnore]
        public IPAddress IP { get; set; } = System.Net.IPAddress.Loopback;

        [XmlElement]
        public string IPAddress
        {
            get { return IP.ToString(); }
            set { IP = System.Net.IPAddress.Parse(value); }
        }

    }
}
