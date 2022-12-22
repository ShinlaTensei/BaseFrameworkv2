using System.Diagnostics;
using System.IO;
using Base.Module;
using NLog;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = NLog.Logger;

namespace Base.Logging
{
    public static class BaseLogSystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void SetupLogSystem()
        {
            #if LOG_ENABLE
            LogManager.ResumeLogging();
            #else
            LogManager.SuspendLogging();
            return;
            #endif
            // Init configuration
            var config = new NLog.Config.LoggingConfiguration();
            
            #if UNITY_EDITOR
            var logConsole = new UnityDebugTarget()
            {
                Name = "UnityDebugLog",
                Layout = "[${level}] ${message} (${stacktrace})",
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
            
            #elif UNITY_ANDROID
            string logFilePath = FileUtilities.GetSystemPath();
            string fileDirectory = Application.productName + "-Debug";
            string fileName = "DebugLog.txt";
            var logFile = new NLog.Targets.FileTarget
            {
                FileName = logFilePath + fileDirectory + "/" + fileName,
                Layout = "[${longdate}] [${level}] ${message} (${stacktrace})"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            CheckOldLog(FileUtilities.GetSystemPath() + Application.productName + "-Debug" + "/" + "DebugLog.txt");
            #elif UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string logFilePath = FileUtilities.GetSystemPath();
            string fileDirectory = Application.productName + "-Debug";
            string fileName = "DebugLog.txt";
            var logFile = new NLog.Targets.FileTarget
            {
                FileName = logFilePath + fileDirectory + "\\" + fileName,
                Layout = "[${longdate}] [${level}] ${message} (${stacktrace})"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            CheckOldLog(FileUtilities.GetSystemPath() + Application.productName + "-Debug" + "\\" + "DebugLog.txt");
            #endif

            LogManager.Configuration = config;
        }
        
        private static void CheckOldLog(string path)
        {
            if (File.Exists(path))
            {
                Debug.Log(path);
                File.Delete(path);
            }
        }
        
        public static Logger GetLogger(this MonoBehaviour target)
        {
            return LogManager.GetCurrentClassLogger();
        }

        public static Logger GetLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
    }
}
