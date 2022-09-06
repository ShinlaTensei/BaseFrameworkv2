using System.Collections;
using System.Collections.Generic;
using NLog;
using NLog.Common;
using NLog.Targets;
using UnityEngine;

namespace Base.Logging
{
    [Target("UnityDebugLog")]
    public class UnityDebugTarget : TargetWithLayout
    {
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(this.Layout, logEvent);
            if (logEvent.Level <= LogLevel.Info)
                UnityEngine.Debug.Log(logMessage);
            else if (logEvent.Level == LogLevel.Warn)
                UnityEngine.Debug.LogWarning(logMessage);
            else
                UnityEngine.Debug.LogError(logMessage);
        }
    }
}

