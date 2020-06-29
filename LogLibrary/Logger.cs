using SettingsLibrary;
using System;
using System.IO;

namespace LogLibrary
{
    public static class Logger
    {
        enum LogType
        {
            ERROR,
            INSERT,
            CREATE,
            EVENT,
            MESSAGE
        }

        public static string LogPath { get; private set; }
        static bool enabled;

        static Logger()
        {
            //settings from xml
            LogPath = Settings.Get<string>(nameof(SettingsProperties.LogPath));
            enabled = Settings.Get<bool>(nameof(SettingsProperties.EnableLogger));
        }

        public static void LogInsert(string message)
        {
            Log(LogType.INSERT, message);
        }
        public static void LogEvent(string message)
        {
            Log(LogType.EVENT, message);
        }

        public static void LogMessage(string message)
        {
            Log(LogType.MESSAGE, message);
        }

        public static void LogCreate(string message)
        {
            Log(LogType.CREATE, message);
        }

        public static void LogError(string message)
        {
            Log(LogType.ERROR, message);
        }

        //logs by using enum and message
        private static void Log(LogType logtype, string message)
        {
            if (enabled)
            {
                string logMessage = $"{DateTime.Now} - {logtype}: {message}\n";

                //appends to file or creates it
                File.AppendAllText(LogPath, logMessage);
            }
        }

    }
}
