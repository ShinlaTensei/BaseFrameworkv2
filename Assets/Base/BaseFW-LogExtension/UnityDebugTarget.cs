using NLog;
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
                Debug.LogFormat($"<b><color=aqua>{logMessage}</color></b>");
            else if (logEvent.Level == LogLevel.Warn)
                Debug.LogWarning($"<b><color=yellow>{logMessage}</color></b>");
            else
                Debug.LogError($"<b><color=red>{logMessage}</color></b>");
        }
    }
}

