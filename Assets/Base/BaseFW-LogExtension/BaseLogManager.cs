using System.IO;
using Base.Module;
using UnityEngine;
using NLog;
using UnityEngine.Android;
using Logger = NLog.Logger;

namespace Base.Logging
{
    public class BaseLogManager : Singleton<BaseLogManager>
    {
        [SerializeField] private bool isDebug = false;
        [SerializeField] private bool isPersistent = false;
        private NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private string _logFilePath;

        private string _fileDirectory;
        private readonly string _fileName = "DebugLog.txt";

        public static Logger BaseLogger => Instance.Logger;
        private void Awake()
        {
            if (isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (isDebug) LogManager.ResumeLogging();
            else LogManager.SuspendLogging();

            _logFilePath = FileUtilities.GetSystemPath();
            _fileDirectory = Application.productName + "-Debug";
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
#elif UNITY_ANDROID
            FileUtilities.RequestPermissionAndroid(new[] {Permission.ExternalStorageRead, Permission.ExternalStorageWrite});
            FileUtilities.CreateFolder(_logFilePath, _fileDirectory);
            var logFile = new NLog.Targets.FileTarget
            {
                FileName = _logFilePath + _fileDirectory + "/" + _fileName,
                Layout = "${longdate} |${level} |${message} |${stacktrace} |${event-properties:myProperty} |${exception}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            CheckOldLog(_logFilePath + _fileDirectory + "/" + _fileName);
#elif UNITY_STANDALONE || UNITY_STANDALONE_WIN
            var logFile = new NLog.Targets.FileTarget
            {
                FileName = _logFilePath + _fileDirectory + "\\" + _fileName,
                Layout = "${longdate} |${level} |${message} |${stacktrace} |${event-properties:myProperty} |${exception}"
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
            CheckOldLog(_logFilePath + _fileDirectory + "\\" + _fileName);
#endif
            
            LogManager.Configuration = config;
        }

        private void CheckOldLog(string path)
        {
            if (File.Exists(path))
            {
                Debug.Log(path);
                File.Delete(path);
            }
        }
    }
}

