using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Base.Helper;
using Base.Logging;
using UnityEngine;

namespace Base.Pattern
{
    public class ServiceLocator : SingletonMono<ServiceLocator>
    {
        private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public Dictionary<Type, IService> Services => _services;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _services.Clear();
        }

        public static T Get<T>() where T : class, IService
        {
            return Resolve<T>();
        }

        public static void Set<T>() where T : class, IService
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                var value = Activator.CreateInstance<T>();
                Instance.Services.Add(typeof(T), value);
            }
            else
            {
                Instance.GetLogger().Debug("Service {0} is already added", typeof(T));
            }
        }

        public static void Set<T>(T argument) where T : class, IService
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                Instance.Services.Add(typeof(T), argument);
            }
            else
            {
                Instance.GetLogger().Debug("Service {0} is already added", typeof(T));
            }
        }

        public static void Remove<T>() where T : class, IService
        {
            if (Instance.Services.ContainsKey(typeof(T)))
            {
                Instance.Services.Remove(typeof(T));
            }
        }

        private static T Resolve<T>() where T : class, IService
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
                    Set((T)result);
                    inst.name = $"{typeof(T).Name}-Singleton";
                }
                else
                {
                    Set<T>();
                    result = Resolve<T>();
                }
            }

            return result as T;
        }
    }
}

