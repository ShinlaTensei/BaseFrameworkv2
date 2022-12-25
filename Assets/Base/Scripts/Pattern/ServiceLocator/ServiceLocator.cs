using System;
using System.Collections.Generic;
using Base.Logging;
using Base.MessageSystem;
using UniRx;
using UnityEngine;

namespace Base.Pattern
{
    public class ServiceLocator : SingletonMono<ServiceLocator>
    {
        private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        private Dictionary<Type, ISignal>   _signals = new Dictionary<Type, ISignal>();

        public Dictionary<Type, IService> Services => _services;
        public Dictionary<Type, ISignal> Signals => _signals;

        protected override void OnDestroy()
        {
            ClearAllListener();
            Services.Clear();
            Signals.Clear();

            base.OnDestroy();
        }

        #region Service

        public static T GetService<T>() where T : class, IService
        {
            return ResolveService<T>();
        }

        public static T SetService<T>() where T : class, IService
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                var value = Activator.CreateInstance<T>();
                Instance.Services.Add(typeof(T), value);

                return value;
            }
            else
            {
                Instance.GetLogger().Debug("Service {0} is already added", typeof(T));

                return null;
            }
        }

        public static T SetService<T>(T argument) where T : class, IService
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                Instance.Services.Add(typeof(T), argument);

                return argument;
            }
            else
            {
                Instance.GetLogger().Debug("Service {0} is already added", typeof(T));

                return null;
            }
        }

        public static void RemoveService<T>() where T : class, IService
        {
            if (Instance.Services.ContainsKey(typeof(T)))
            {
                Instance.Services.Remove(typeof(T));
            }
        }

        private static T ResolveService<T>() where T : class, IService
        {
            IService result = default;
            if (Instance.Services.TryGetValue(typeof(T), out IService concreteType))
            {
                result = concreteType;
            }
            else
            {
                if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
                {
                    GameObject inst = new GameObject();
                    inst.transform.SetParent(Instance.CacheTransform);
                    result = inst.AddComponent(typeof(T)) as T;
                    SetService((T)result);
                    inst.name = $"{typeof(T).Name}-Singleton";
                }
                else
                {
                    SetService<T>();
                    result = ResolveService<T>();
                }
            }

            return result as T;
        }

        #endregion

        #region Signal

        public static T GetSignal<T>() where T : class, ISignal
        {
            return ResolveSignal<T>();
        }

        public static void SetSignal<T>() where T : class, ISignal
        {
            if (!Instance.Signals.ContainsKey(typeof(T)))
            {
                T signal = Activator.CreateInstance<T>();
                Instance.Signals.TryAdd(typeof(T), signal);
            }
            else
            {
                Instance.GetLogger().Debug("Signal {0} is already added", typeof(T));
            }
        }

        public static void RemoveSignal<T>() where T : class, ISignal
        {
            if (Instance.Signals.ContainsKey(typeof(T)))
            {
                Instance.Signals.Remove(typeof(T));
            }
        }

        private static T ResolveSignal<T>() where T : class, ISignal
        {
            ISignal result = default;
            if (Instance.Signals.TryGetValue(typeof(T), out ISignal concreteSignal))
            {
                result = concreteSignal;
            }
            else
            {
                SetSignal<T>();
                result = ResolveSignal<T>();
            }

            return result as T;
        }

        private void ClearAllListener()
        {
            foreach (var signal in Signals.Values)
            {
                signal.RemoveAllListener();
            }
        }

        #endregion
    }
}

