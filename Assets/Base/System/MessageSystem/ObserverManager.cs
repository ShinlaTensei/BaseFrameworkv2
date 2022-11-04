using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.MessageSystem
{
    public class ObserverManager : SingletonMono<ObserverManager>
    {
        private Dictionary<Enum, Delegate> _listeners = new Dictionary<Enum, Delegate>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _listeners.Clear();
        }

        public void AddListener<T>(Enum eventId, Callback<T> func)
        {
            Debug.AssertFormat(func != null, "AddListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});
            if (_listeners.ContainsKey(eventId))
            {
                _listeners[eventId] = func;
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

        public void RemoveListener<T>(Enum eventId, Callback<T> func)
        {
            Debug.AssertFormat(func != null, "RemoveListener event {0} failed due to callback is null !!!", new object[] {eventId.ToString()});

            if (_listeners.ContainsKey(eventId))
            {
                _listeners.Remove(eventId);
            }
            else
            {
                Debug.LogErrorFormat("Event {0} not found", new object[] {eventId.ToString()});
            }
        }

        public void BroadcastEvent<T>(Enum eventId, T argument)
        {
            if (!_listeners.ContainsKey(eventId))
            {
                Debug.LogErrorFormat("No one subscribe for event {0}", new object[] {eventId.ToString()});
                return;
            }

            var callback = _listeners[eventId];
            if (callback != null)
            {
                ((Callback<T>) callback).Invoke(argument);
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
        public static void RegisterListener<T>(this MonoBehaviour target, Enum eventId, Callback<T> func)
        {
            ObserverManager.Instance.AddListener(eventId, func);
        }

        public static void RemoveListener<T>(this MonoBehaviour target, Enum eventId, Callback<T> func)
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

        public static void PostEvent<T>(this MonoBehaviour target, Enum eventId, T argument)
        {
            ObserverManager.Instance.BroadcastEvent(eventId, argument);
        }
    }
}
