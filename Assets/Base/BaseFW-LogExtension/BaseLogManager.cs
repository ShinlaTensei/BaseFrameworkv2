using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NLog;
using Logger = NLog.Logger;

namespace Base.Logging
{
    public class BaseLogManager : Singleton<BaseLogManager>
    {
        private NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private string _logFilePath;

        private readonly string _fileName = "/DebugLog.txt";

        public static Logger BaseLogger => Instance.Logger;
        private void Awake()
        {
            _logFilePath = Application.persistentDataPath;
            CheckOldLog();
            InitLogConfiguration();
        }

        private void InitLogConfiguration()
        {
            // Init configuration
            var config = new NLog.Config.LoggingConfiguration();

            var logFile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = _logFilePath + _fileName,
                Layout = "${longdate}|${level}|${message}|${exception}|${event-properties:myProperty}"
            };
            var logConsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            
            LogManager.Configuration = config;
        }

        private void CheckOldLog()
        {
            if (File.Exists(_logFilePath + _fileName))
            {
                File.Delete(_logFilePath + _fileName);
            }
        }
    }
}

