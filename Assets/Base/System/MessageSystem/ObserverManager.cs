using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.MessageSystem
{
    public class ObserverManager : SingletonMono<ObserverManager>
    {
        private Dictionary<Enum, Callback<object>> _listeners = new Dictionary<Enum, Callback<object>>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _listeners.Clear();
        }

        public void AddListener(Enum eventId, Callback<object> func)
        {
            Debug.AssertFormat(func != null, "AddListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});
            if (_listeners.ContainsKey(eventId))
            {
                _listeners[eventId] += func;
            }
            else
            {
                if (_listeners.TryAdd(eventId, func))
                {
                    
                }
                else
                {
                    _listeners.Add(eventId, null);
                    _listeners[eventId] = func;
                }
            }
        }

        public void RemoveListener(Enum eventId, Callback<object> func)
        {
            Debug.AssertFormat(func != null, "RemoveListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});

            if (_listeners.ContainsKey(eventId))
            {
                _listeners[eventId] -= func;
            }
            else
            {
                Debug.LogErrorFormat("Event {0} not found", new object[] {eventId.ToString()});
            }
        }

        public void BroadcastEvent(Enum eventId, object argument)
        {
            if (!_listeners.ContainsKey(eventId))
            {
                Debug.LogErrorFormat("No one subscribe for event {0}", new object[] {eventId.ToString()});
                return;
            }

            var callback = _listeners[eventId];
            if (callback != null)
            {
                callback.Invoke(argument);
            }
            else
            {
                Debug.LogErrorFormat("Event {0} has zero listener", new object[] {eventId.ToString()});
                _listeners.Remove(eventId);
            }
        }
    }

    public static class ObserverExtension
    {
        public static void RegisterListener(this MonoBehaviour target, Enum eventId, Callback<object> func)
        {
            ObserverManager.Instance.AddListener(eventId, func);
        }

        public static void RemoveListener(this MonoBehaviour target, Enum eventId, Callback<object> func)
        {
            try
            {
                ObserverManager.Instance.RemoveListener(eventId, func);
            }
            catch (Exception e)
            {
                Debug.LogException(exception: e);
            }
        }

        public static void PostEvent(this MonoBehaviour target, Enum eventId, object argument)
        {
            ObserverManager.Instance.BroadcastEvent(eventId, argument);
        }
    }
}
