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
        
        [SerializeField] private GameObject m_root;
        [SerializeField] private ExitType m_exitType;
        [SerializeField] private UICanvasType m_canvasType;
        [BitFlag(typeof(NavigationState))]
        [SerializeField] private long m_navigationState = 0;
        [SerializeField] private bool m_activeDefault;
        [SerializeField] private bool m_closePrevOnShow;
        [SerializeField] private bool m_closeOnTouchOutside;
        [SerializeField] private bool m_triggerViewChange;
        [Condition("m_closeOnTouchOutside", true, false)] 
        [SerializeField] private RectTransform m_touchRect;

        private IViewData m_data = null;

        private bool m_isShow = false;

        public GameObject Root
        {
            get
            {
                if (!m_root)
                {
                    m_root = CacheTransform.FindChildRecursive<GameObject>(RootName);
                }
                return m_root;
            }
        }

        public ExitType ExitType => m_exitType;
        public UICanvasType CanvasType => m_canvasType;
        public bool ActiveDefault => m_activeDefault;
        public bool ClosePrevOnShow => m_closePrevOnShow;
        public bool TriggerViewChange => m_triggerViewChange;
        public long NavigationState => m_navigationState;
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
            switch (m_exitType)
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
            if (!m_closeOnTouchOutside) return;
            bool inside = false;
            if (m_touchRect)
            {
                inside = RectTransformUtility.RectangleContainsScreenPoint(m_touchRect, eventData.position, eventData.pressEventCamera);
            }
            
            if(!inside) {Hide();}
        }
    }
}

