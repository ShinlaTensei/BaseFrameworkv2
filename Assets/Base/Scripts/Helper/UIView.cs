using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Pattern;
using Base.Utilities;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public enum ExitType {None, Hide, Remove, RemoveImmediate}
    public enum NavigationState {
        None = 0,
        Obscured,
        Overlap}
    public abstract class UIView : BaseMono, IPointerClickHandler
    {
        private const string RootName = "Root";
        
        [SerializeField] private GameObject root;
        [SerializeField] private ExitType exitType;
        [SerializeField] private UICanvasType canvasType;
        [BitFlag(typeof(NavigationState))]
        [SerializeField] private long navigationState = 0;
        [SerializeField] private bool activeDefault;
        [SerializeField] private bool closePrevOnShow;
        [SerializeField] private bool closeOnTouchOutside;
        [Condition("closeOnTouchOutside", true, false)] 
        [SerializeField] private RectTransform touchRect;

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

        public long NavigationState => navigationState;

        public virtual void Show()
        {
            //if (IsMissingReference) return;
            
            Root.SetActive(true);
        }

        public virtual void Hide()
        {
            //if (IsMissingReference) return;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!closeOnTouchOutside) return;
            bool inside = false;
            if (touchRect)
            {
                inside = RectTransformUtility.RectangleContainsScreenPoint(touchRect, eventData.position, eventData.pressEventCamera);
            }
            
            if(!inside) {Hide();}
        }
    }
}

