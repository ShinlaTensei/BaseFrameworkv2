using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public delegate void IntervalDelegate(IntervalConfig config);

    public class IntervalConfig
    {
        public bool isCancel = false;
        public bool isDone = false;
        public MonoBehaviour caller;
        public IEnumerator action = null;

        public void Start()
        {
            if (caller != null && action != null)
                caller.StartCoroutine(action);
        }

        public void Stop()
        {
            if (caller != null && action != null)
            {
                caller.StopCoroutine(action);
            }
        }
    }
    public static class BaseInterval
    {
        
        /// <summary>
        /// Run a function after an amount of time
        /// </summary>
        /// <param name="caller"> The caller object </param>
        /// <param name="time"></param>
        /// <param name="callback"></param>
        /// <param name="config"></param>
        public static IntervalConfig RunAfterTime(MonoBehaviour caller, float time, IntervalDelegate callback)
        {
            IntervalConfig interval = new IntervalConfig();

            interval.action = _RunAfterTime(interval, time, callback);
            interval.caller = caller;
            interval.Start();
            return interval;
        }
        
        private static IEnumerator _RunAfterTime(IntervalConfig config, float _time, IntervalDelegate _callback)
        {
            yield return new WaitForSeconds(_time);
            if (!config.isCancel)
            {
                _callback.Invoke(config);
                config.isDone = true;
            }
        }
        
        /// <summary>
        /// Run a function at end of frame
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="callback"></param>
        public static IntervalConfig RunAtEndOfFrame(MonoBehaviour caller, IntervalDelegate callback)
        {
            IntervalConfig config = new IntervalConfig();
            config.caller = caller;
            config.action = _RunAtEndOfFrame(config, callback);
            config.Start();
            return config;
        }
        
        private static IEnumerator _RunAtEndOfFrame(IntervalConfig config, IntervalDelegate callback)
        {
            yield return new WaitForEndOfFrame();

            if (!config.isCancel)
            {
                callback.Invoke(config);
                config.isDone = true;
            }
        }
        
        /// <summary>
        /// Run a function at every frame
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="initialDelay"></param>
        /// <param name="timeDelay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IntervalConfig RunAtEveryFrame(MonoBehaviour caller,float initialDelay, float timeDelay, IntervalDelegate callback)
        {
            IntervalConfig config = new IntervalConfig();
            config.caller = caller;
            config.action = _RunAtEveryFrame(config, initialDelay, timeDelay, callback);
            config.Start();
            return config;
        }

        private static IEnumerator _RunAtEveryFrame(IntervalConfig config, float initialDelay, float timeDelay, 
            IntervalDelegate callback)
        {
            if (initialDelay > 0)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            while (true)
            {
                if (!config.isCancel)
                {
                    callback.Invoke(config);
                }
                else
                {
                    break;
                }

                if (timeDelay > 0)
                {
                    yield return new WaitForSeconds(timeDelay);
                }
            }

            config.isDone = true;
        }

        public static IntervalConfig RunLerp(MonoBehaviour caller, float duration, Action<float> callback)
        {
            IntervalConfig config = new IntervalConfig();
            config.caller = caller;
            config.action = _RunLerp(config, duration, callback);
            config.Start();
            return config;
        }

        private static IEnumerator _RunLerp(IntervalConfig config, float duration, Action<float> callback)
        {
            config.isDone = false;
            float t = 0f;
            while (t < 1.0f)
            {
                t = Mathf.Clamp01(t + Time.deltaTime / duration);
                if (!config.isCancel)
                    callback.Invoke(t);
                yield return null;
            }
            config.isDone = true;
        }
    }
}

