using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Base.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public enum UICanvasType
    {
        None            = 0,
        RootCanvas      = 1,
        ViewCanvas      = 2,
        TopViewCanvas   = 3,
        OverlayCanvas   = 4,
        RetryCanvasUI   = 5,
        UIOverlayLayout = 6,
        CampaignCanvasUI = 7,
    }
        
    public class OnUIViewChangedSignal : Signal<UIView, UIView> {}
    public class UIViewManager : BaseMono, IService
    {
        private const string RootName = "Root";

        #region UIView Handle
        
        private Dictionary<string, UIView>  m_uiViewPool    = new Dictionary<string, UIView>();
        private List<UIView>               m_stackUi       = new List<UIView>();

        private UIView m_previous;
        private UIView m_current;

        private AddressableManager m_addressableManager;

        public UIView Previous => m_previous;

        private void NotifyUIViewChanged()
        {
            if (m_previous.TriggerViewChange)
            {
                ServiceLocator.GetSignal<OnUIViewChangedSignal>()?.Dispatch(m_previous, m_current);
            }
        }
        private async UniTask<T> ShowAsync<T>(T instance, Action<T> onInit = null, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T view = instance ? instance : GetView<T>();

            if (!view)
            {
                view = await InitAsync<T>(null, cancellationToken);
            }

            if (view)
            {
                await InitCompleted(view, root, cancellationToken);
            }
            
            onInit?.Invoke(view);

            return view;
        }
        
        public async UniTask<T> Show<T>(T instance, IViewData viewData = null, Action<T> onInit = null, 
            Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(instance, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);

            return inst;
        }
        
        public async UniTask<T> Show<T, Y>(T instance, Y value, Action<T> onInit = null, Transform root = null,
            CancellationToken cancellationToken = default) where T : UIView where Y : IViewData
        {
            return await ShowAsync<T>(instance, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
        }

        public async UniTask<T> Show<T>(Action<T> onInit = null, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);

            return inst;
        }

        public async UniTask<T1> Show<T1, T2>(T2 value, Action<T1> onInit = null, Transform root = null,
            CancellationToken cancellationToken = default) where T1 : UIView where T2 : IViewData
        {
            T1 inst = await ShowAsync<T1>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);

            return inst;
        }

        public void CloseView<T>(T view) where T : UIView
        {
            if (view is null) return;

            if (m_uiViewPool.ContainsValue(view))
            {
                if (view.ExitType is ExitType.Remove or ExitType.RemoveImmediate) Remove(view);
                view.Hide();

                NotifyUIViewChanged();
            }
        }

        private UIView GetTopUIView()
        {
            for (int i = 0; i < m_stackUi.Count; ++i)
            {
                UIView view = m_stackUi[i];
                if (view && view.IsShowing)
                {
                    return view;
                }
            }

            return null;
        }

        private void Add<T>(T view) where T : UIView
        {
            if (!m_uiViewPool.ContainsKey(typeof(T).Name))
            {
                m_uiViewPool.TryAdd(typeof(T).Name, view);
            }
        }

        public void Remove<T>(T value) where T : UIView
        {
            if (m_uiViewPool.ContainsKey(typeof(T).Name))
            {
                m_uiViewPool.Remove(typeof(T).Name);
            }

            if (m_stackUi.Contains(value))
            {
                m_stackUi.Remove(value);
            }
        }
        
        public T GetView<T>() where T : UIView
        {
            m_uiViewPool.TryGetValue(typeof(T).Name, out UIView value);

            return value as T;
        }

        private void PushStack<T>(T view) where T : UIView
        {
            if (view != null)
            {
                m_stackUi.Remove(view);
                m_stackUi.Insert(0, view);
            }
            PDebug.InfoFormat("[UIViewManager] Push {0}", view.GetType().Name);
        }

        private async UniTask<T> InitAsync<T>(Action<T> onCompleted = null, CancellationToken cancellationToken = default) where T : UIView
        {
            UIModelAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(UIModelAttribute)) as UIModelAttribute;

            if (attribute == null)
            {
                PDebug.ErrorFormat("[UIView]Need to apply UIModelAttribute on class {name}", typeof(T).Name);

                return null;
            }

            string modelName = attribute.ModelName;

            GameObject inst = null;
            string prefabPath = string.Empty;

            if (m_addressableManager.IsInit && m_addressableManager.IsReadyToGetBundle)
            {
                prefabPath = modelName;
                inst = await m_addressableManager.InstantiateAsync(prefabPath,
                    parent: GetCanvasWithTag(UICanvasType.RootCanvas, attribute.SceneName), retryCount: 5,
                    cancellationToken: cancellationToken);
            }

            T view = inst != null ? inst.GetComponent<T>() : null;
            onCompleted?.Invoke(view);

            return view;
        }

        private async UniTask InitCompleted<T>(T instance, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            if (instance == null)
            {
                PDebug.ErrorFormat("[UIView]Null reference of type {type}", typeof(T).Name);

                return;
            }

            Transform parent = root != null ? root : GetCanvasWithTag(instance.CanvasType, instance.CacheGameObject.scene.name);
            if (parent)
            {
                instance.CacheTransform.SetParent(parent, false);
            }
            
            instance.CacheTransform.SetScale(1);
            instance.CacheTransform.SetLocalPosition(Vector3.zero);
            instance.RectTransform.anchoredPosition = Vector3.zero;
            if (instance.NavigationState.HasBit(NavigationState.Overlap))
            {
                instance.CacheTransform.SetAsFirstSibling();
            }
            else
            {
                instance.CacheTransform.SetAsLastSibling();
            }
            instance.Root.SetActive(instance.ActiveDefault);
            
            m_previous = m_current;
            m_current = instance;
            await m_current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
            m_current.Show();
            if (m_previous && m_current.ClosePrevOnShow) CloseView(m_previous);

            Add(instance);
            PushStack(instance);
        }

        #endregion

        #region Canvas Handle
        
        private Dictionary<string, Transform> m_uiCanvasPool = new Dictionary<string, Transform>();

        public Transform GetCanvasWithTag(UICanvasType enumTag)
        {
            string newTag = !(enumTag is UICanvasType.None) ? enumTag.ToString() : UICanvasType.RootCanvas.ToString();

            if (m_uiCanvasPool.TryGetValue(newTag, out Transform value))
            {
                return value;
            }

            GameObject obj = GameObject.FindGameObjectWithTag(newTag);
            if (obj != null)
            {
                m_uiCanvasPool.TryAdd(newTag, obj.transform);

                return obj.transform;
            }

            return null;
        }
        
        public Transform GetCanvasWithTag(UICanvasType enumTag, string sceneName)
        {
            string newTag = !(enumTag is UICanvasType.None) ? enumTag.ToString() : UICanvasType.RootCanvas.ToString();
            Scene scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.isLoaded) return null;

            string key = $"{newTag}-{scene.name}";
            if (m_uiCanvasPool.TryGetValue(key, out Transform result))
            {
                return result;
            }

            GameObject[] objects = GameObject.FindGameObjectsWithTag(newTag);
            for (int i = 0; i < objects.Length; ++i)
            {
                if (objects[i].scene.name.Equals(scene.name, StringComparison.CurrentCulture))
                {
                    Transform final = objects[i].transform.FindChildRecursive<Transform>(RootName);
                    m_uiCanvasPool.TryAdd($"{newTag}-{scene.name}", final);

                    return final;
                }
            }
            
            PDebug.WarnFormat("Cannot find canvas with tag {tag} in scene: {sceneName}", newTag, sceneName);

            return null;
        }
        
        public void Remove(UICanvasType canvasType)
        {
            string key = canvasType.ToString();
            if (m_uiCanvasPool.ContainsKey(key))
            {
                m_uiCanvasPool.Remove(key);
            }
        }

        public void Remove(UICanvasType canvasType, string sceneName)
        {
            string key = $"{canvasType.ToString()}-{sceneName}";
            if (m_uiCanvasPool.ContainsKey(key))
            {
                m_uiCanvasPool.Remove(key);
            }
        }

        #endregion

        public void Init()
        {
            m_addressableManager = ServiceLocator.GetService<AddressableManager>();
        }

        public void DeInit()
        {
            m_uiCanvasPool.Clear();
            m_uiViewPool.Clear();
            m_stackUi.Clear();
        }
    }
}

