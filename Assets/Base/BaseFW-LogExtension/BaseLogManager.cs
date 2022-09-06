using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NLog;
using ILogger = UnityEngine.ILogger;
using Logger = NLog.Logger;

namespace Base.Logging
{
    public class BaseLogManager : Singleton<BaseLogManager>
    {
        [SerializeField] private bool isDebug = false;
        [SerializeField] private bool isPersistent = false;
        private NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private string _logFilePath;

        private readonly string _fileName = "/DebugLog.txt";

        public static Logger BaseLogger => Instance.Logger;
        private void Awake()
        {
            if (isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (isDebug) LogManager.ResumeLogging();
            else LogManager.SuspendLogging();
            
            _logFilePath = Application.persistentDataPath;
            CheckOldLog();
            InitLogConfiguration();
        }

        private void InitLogConfiguration()
        {
            // Init configuration
            var config = new NLog.Config.LoggingConfiguration();

#if UNITY_EDITOR
            var logConsole = new UnityDebugTarget()
            {
                Name = "UnityDebugLog",
                Layout = "${longdate} |${level} |${message} |${stacktrace}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
#else
            var logFile = new NLog.Targets.FileTarget
            {
                FileName = _logFilePath + _fileName,
                Layout = "${longdate} |${level} |${message} |${stacktrace} |${event-properties:myProperty} |${exception}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
#endif
            
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

