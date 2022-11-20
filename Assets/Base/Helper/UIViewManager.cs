using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public class UIViewManager : SingletonMono<UIViewManager>
    {
        private Dictionary<string, UIView> _uiInterfaces = new Dictionary<string, UIView>();

        public void Install<T>(T uiView) where T : UIView
        {
            if (!_uiInterfaces.TryGetValue(typeof(T).Name, out UIView value))
            {
                var fullName = typeof(T).Name;
                _uiInterfaces[fullName] = uiView;
            }
        }

        public void UnInstall<T>(T uiView) where T : UIView
        {
            if (_uiInterfaces.TryGetValue(typeof(T).Name, out UIView value))
            {
                _uiInterfaces.Remove(typeof(T).Name);
            }
        }

        public T GetView<T>() where T : UIView
        {
            if (_uiInterfaces.TryGetValue(typeof(T).Name, out UIView value))
            {
                return value as T;
            }

            return null;
        }
    }
}

