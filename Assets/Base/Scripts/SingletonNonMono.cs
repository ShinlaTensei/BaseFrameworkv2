using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public abstract class SingletonNonMono<T> where T : class
    {
        private static T _instance;
        
        protected static bool m_ShuttingDown = false;

        private static object m_Lock = new object();

        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (_instance == null)
                    {
                        _instance = Activator.CreateInstance<T>();
                    }
                    return _instance;
                }
                
            }
        }

        ~SingletonNonMono()
        {
            m_ShuttingDown = true;
            _instance = null;
        }
    }
}

