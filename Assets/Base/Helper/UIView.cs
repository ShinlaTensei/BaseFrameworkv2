using System;
using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;

namespace Base.Helper
{
    public enum ExitType {None, Hide, Remove}
    public abstract class UIView : BaseMono, IService
    {
        [SerializeField] protected GameObject root;
        [SerializeField] protected ExitType exitType;

        public virtual void Show()
        {
            if (isMissingReference) return;
            
            root.SetActive(true);
        }

        public virtual void Hide()
        {
            if (isMissingReference) return;

            switch (exitType)
            {
                case ExitType.Hide:
                    root.SetActive(false);
                    break;
                case ExitType.Remove:
                    Destroy(CacheGameObject);
                    break;
                default: break;
            }
        }
        
        public abstract void Next();
        public abstract void Back();

        public virtual void Show<T>(T argument) {}
        public virtual void Hide<T>(T argument) {}
    }
}

