using System.Threading;
using Base.Pattern;
using Base.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public enum ExitType {None, Hide, Remove, RemoveImmediate}
    public enum NavigationState {
        None = 0,
        Obscured,
        Overlap}
    
    public interface IViewData {}
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
        [SerializeField] private bool triggerViewChange;
        [Condition("closeOnTouchOutside", true, false)] 
        [SerializeField] private RectTransform touchRect;

        private IViewData _data = null;

        private bool m_isShow = false;

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
        public bool TriggerViewChange => triggerViewChange;
        public long NavigationState => navigationState;
        public bool IsShowing { get => m_isShow; set => m_isShow = value; }

        public virtual void Show()
        {
            //if (IsMissingReference) return;
            
            Root.SetActive(true);
            IsShowing = true;
        }

        public virtual void Hide()
        {
            //if (IsMissingReference) return;
            IsShowing = false;
            switch (exitType)
            {
                case ExitType.Hide:
                    Root.SetActive(false);
                    break;
                case ExitType.Remove:
                    ServiceLocator.GetService<UIViewManager>()?.Remove(this);
                    Destroy(CacheGameObject);
                    break;
                case ExitType.RemoveImmediate:
                    ServiceLocator.GetService<UIViewManager>()?.Remove(this);
                    DestroyImmediate(CacheGameObject);
                    break;
                default: break;
            }
        }
        
        public virtual void Next() {}

        public virtual void Back()
        {
            UIViewManager viewManager = ServiceLocator.GetService<UIViewManager>();
            if (viewManager && viewManager.Previous)
            {
                viewManager.Show(viewManager.Previous).Forget();
            }
        }

        public virtual void Show<T>(T argument) where T : IViewData
        {
            Show();
            
            Populate(argument);
        }

        public virtual void Hide<T>(T argument) where T : IViewData
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
        
        public virtual void Populate<T>(T viewData) where T : IViewData {}

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

