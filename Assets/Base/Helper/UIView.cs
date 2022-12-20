using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Helper
{
    public enum ExitType {None, Hide, Remove, RemoveImmediate}
    public abstract class UIView : BaseMono
    {
        private const string RootName = "Root";
        
        [SerializeField] private GameObject root;
        [SerializeField] private ExitType exitType;
        [SerializeField] private UICanvasType canvasType;
        [SerializeField] private bool activeDefault;
        [SerializeField] private bool closePrevOnShow;

        public GameObject Root
        {
            get
            {
                if (!root)
                {
                    root = CacheTransform.FindChildRecursive<GameObject>(RootName);
                }
                return root;
            }
        }

        public UICanvasType CanvasType => canvasType;
        public bool ActiveDefault => activeDefault;
        public bool ClosePrevOnShow => closePrevOnShow;

        public virtual void Show()
        {
            if (isMissingReference) return;
            
            Root.SetActive(true);
        }

        public virtual void Hide()
        {
            if (isMissingReference) return;

            switch (exitType)
            {
                case ExitType.Hide:
                    Root.SetActive(false);
                    break;
                case ExitType.Remove:
                    Destroy(CacheGameObject);
                    break;
                case ExitType.RemoveImmediate:
                    DestroyImmediate(CacheGameObject);
                    break;
                default: break;
            }
        }
        
        public virtual void Next() {}
        public virtual void Back() {}

        public virtual void Show<T>(T argument)
        {
            Show();
        }

        public virtual void Hide<T>(T argument)
        {
            Hide();
        }
        
        /// <summary>
        /// Wait for something on transition of UI
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>UniTask</returns>
        public virtual async UniTask Await(CancellationToken cancellationToken = default)
        {
            if (false)
            {
                await UniTask.Yield();
            }
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.GetService<UIViewManager>().Remove(this);
        }
    }
}

