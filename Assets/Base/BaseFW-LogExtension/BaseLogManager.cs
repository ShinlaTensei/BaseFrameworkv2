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

        #region Info Overload

        public void Info<T>(T message)
        {
            if (isDebug)
            {
                Logger.Info(message);
            }
        }

        public void Info(IFormatProvider formatProvider, string message, string[] arguments)
        {
            if (isDebug)
            {
                Logger.Info(formatProvider, message, arguments);
            }
        }
        
        public void Info<T>(IFormatProvider formatProvider, T value)
        {
            if (isDebug)
            {
                Logger.Info(formatProvider, value);
            }
        }

        #endregion
        
        #region Debug Overload

        /// <overloads>
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Debug<T>(T value)
        {
            if (isDebug)
            {
                Logger.Debug(value);
            }
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Debug<T>(IFormatProvider formatProvider, T value)
        {
            if (isDebug)
            {
                Logger.Debug(formatProvider, value);
            }
        }

        #endregion

        #region Warn Overload

        /// <overloads>
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Warn<T>(T value)
        {
            if (isDebug)
            {
                Logger.Warn(value);
            }
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Warn<T>(IFormatProvider formatProvider, T value)
        {
            if (isDebug)
            {
                Logger.Warn(formatProvider, value);
                
            }
        }

        #endregion

        #region Error Overload

        // <overloads>
        /// Writes the diagnostic message at the <c>Error</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Error<T>(T value)
        {
            if (isDebug)
            {
                Logger.Error(value);
            }
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Error<T>(IFormatProvider formatProvider, T value)
        {
            if (isDebug)
            {
                Logger.Error(formatProvider, value);
            }
        }

        #endregion
        
    }
}

