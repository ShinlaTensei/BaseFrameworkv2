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

        public static T Get<T>() where T : class, IService, new()
        {
            if (Instance.Services.TryGetValue(typeof(T), out IService service))
            {
                return service as T;
            }
            else
            {
                var result = ConstructService<T>();
                Instance.Services.Add(typeof(T), result);
                return result;
            }
        }

        public static void Set<T>() where T : class, IService, new()
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                var value = new T();
                Instance.Services.Add(typeof(T), value);
            }
            else
            {
                Instance.GetLogger().Debug("Service {0} is already added", typeof(T));
            }
        }

        public static void Set<T>(T argument) where T : IService
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

        private static T ConstructService<T>() where T : class, IService, new()
        {
            var attribute = typeof(T).GetCustomAttribute(typeof(UIModelAttribute)) as UIModelAttribute;
            if (ReferenceEquals(attribute, null))
            {
                T value = new T();
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}

