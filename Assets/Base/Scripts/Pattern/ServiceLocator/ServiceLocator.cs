using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Base.Module;
using Base.Services;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Base.Pattern
{
    public class ServiceLocator : SingletonMono<ServiceLocator>
    {
        private Dictionary<Type, IService> m_services = new Dictionary<Type, IService>();
        private Dictionary<Type, ISignal>   m_signals = new Dictionary<Type, ISignal>();
        private Dictionary<Type, IBlueprint> m_blueprints = new Dictionary<Type, IBlueprint>();

        public Dictionary<Type, IBlueprint> Blueprints => m_blueprints;
        public Dictionary<Type, IService> Services => m_services;
        public Dictionary<Type, ISignal> Signals => m_signals;

        protected override void OnDestroy()
        {
            ClearAllListener();
            foreach (var service in Services.Values)
            {
                try
                {
                    service.DeInit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Services.Clear();
            Signals.Clear();
            Blueprints.Clear();
            
            base.OnDestroy();
        }

        #region Service
        
        [CanBeNull]
        public static T GetService<T>() where T : class, IService
        {
            return ResolveService<T>();
        }

        public static T SetService<T>() where T : class, IService
        {
            if (!Instance.Services.ContainsKey(typeof(T)))
            {
                IService result;
                if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
                {
                    GameObject inst = new GameObject();
                    inst.transform.SetParent(Instance.CacheTransform);
                    result = inst.AddComponent(typeof(T)) as T;
                    inst.name = $"{typeof(T).Name}-Singleton";
                }
                else
                {
                    result = Activator.CreateInstance<T>();
                }
                
                Instance.Services.Add(typeof(T), result);

                return (T)result;
            }
            else
            {
                PDebug.InfoFormat("Service {0} is already added", typeof(T));

                return GetService<T>();
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
                PDebug.InfoFormat("Service {0} is already added", typeof(T));

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
            if (ShuttingDown) return null;
            
            IService result = default;
            result = Instance.Services.TryGetValue(typeof(T), out IService concreteType) ? concreteType : SetService<T>();

            return result as T;
        }

        #endregion

        #region Signal
        
        [CanBeNull]
        public static T GetSignal<T>() where T : class, ISignal
        {
            return ResolveSignal<T>();
        }

        public static T SetSignal<T>() where T : class, ISignal
        {
            if (!Instance.Signals.ContainsKey(typeof(T)))
            {
                T signal = Activator.CreateInstance<T>();
                Instance.Signals.TryAdd(typeof(T), signal);

                return signal;
            }
            
            {
                PDebug.InfoFormat("Signal {0} is already added", typeof(T));

                return GetSignal<T>();
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
            if (ShuttingDown) return null;
            
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
        
        #region Blueprint
        
        [CanBeNull]
        public static T GetBlueprint<T>() where T : class, IBlueprint
        {
            return ResolveBlueprint<T>();
        }

        private static T ResolveBlueprint<T>() where T : class, IBlueprint
        {
            if (ShuttingDown) return null;
            
            IBlueprint result = default;
            if (Instance.Blueprints.TryGetValue(typeof(T), out IBlueprint concreteSignal))
            {
                result = concreteSignal;
            }
            else
            {
                SetBlueprint<T>();
                result = ResolveBlueprint<T>();
            }

            return result as T;
        }
        
        public static T SetBlueprint<T>() where T : class, IBlueprint
        {
            if (!Instance.Blueprints.ContainsKey(typeof(T)))
            {
                T blueprint = Activator.CreateInstance<T>();
                Instance.Blueprints.TryAdd(typeof(T), blueprint);

                return blueprint;
            }
            
            {
                PDebug.InfoFormat("Signal {0} is already added", typeof(T));

                return GetBlueprint<T>();
            }
        }
        
        public static void RemoveBlueprint<T>() where T : class, IBlueprint
        {
            if (Instance.Blueprints.ContainsKey(typeof(T)))
            {
                Instance.Blueprints.Remove(typeof(T));
            }
        }

        #endregion

        public IList<T> GetAll<T>() where T : class
        {
            IList<T> types = new List<T>();
            foreach (var service in m_services)
            {
                Type t = service.Key;
                IService ins = service.Value;
                if (ins is T obj)
                {
                    types.AddIfNotContains(obj);
                }
            }

            foreach (var blueprint in m_blueprints)
            {
                IBlueprint bp = blueprint.Value;
                if (bp is T output)
                {
                    types.AddIfNotContains(output);
                }
            }

            return types;
        }
    }
}

