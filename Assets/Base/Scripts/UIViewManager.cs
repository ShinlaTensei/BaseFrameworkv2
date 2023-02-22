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
        
        private Dictionary<string, UIView> _uiViewPool = new Dictionary<string, UIView>();
        
        private UIView _previous;
        private UIView _current;

        private AddressableManager _addressableManager;

        public UIView Previous => _previous;

        private void NotifyUIViewChanged()
        {
            if (_previous.TriggerViewChange)
            {
                ServiceLocator.GetSignal<OnUIViewChangedSignal>()?.Dispatch(_previous, _current);
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
                InitCompleted(view, root);
            }
            
            onInit?.Invoke(view);

            return view;
        }
        
        public async UniTask<T> Show<T>(T instance, IViewData viewData = null, Action<T> onInit = null, 
            Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(instance, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                _current.Populate(viewData);
                _current.Show();
                if (_previous && _current.ClosePrevOnShow) _previous.Hide();
            }

            return inst;
        }
        
        public async UniTask Show<T, Y>(T instance, Y value, Action<T> onInit = null, Transform root = null,
            CancellationToken cancellationToken = default) where T : UIView where Y : IViewData
        {
            T inst = await ShowAsync<T>(instance, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                if (_previous && _current.ClosePrevOnShow)
                {
                    _previous.Hide();
                }
                _current.Show(value);
                NotifyUIViewChanged();
            }
        }

        public async UniTask<T> Show<T>(Action<T> onInit = null, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                _current.Show();
                NotifyUIViewChanged();
                if (_previous && _current.ClosePrevOnShow) _previous.Hide();
            }

            return inst;
        }

        public async UniTask<T1> Show<T1, T2>(T2 value, Action<T1> onInit = null, Transform root = null,
            CancellationToken cancellationToken = default) where T1 : UIView where T2 : IViewData
        {
            T1 inst = await ShowAsync<T1>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                _current.Show(value);
                NotifyUIViewChanged();
                if (_previous && _current.ClosePrevOnShow) _previous.Hide();
            }

            return inst;
        }
        
        public void Add<T>(T view) where T : UIView
        {
            if (!_uiViewPool.ContainsKey(typeof(T).Name))
            {
                _uiViewPool.TryAdd(typeof(T).Name, view);
            }
        }

        public void Remove<T>(T value) where T : UIView
        {
            if (_uiViewPool.ContainsKey(typeof(T).Name))
            {
                _uiViewPool.Remove(typeof(T).Name);
            }
        }
        
        public T GetView<T>() where T : UIView
        {
            _uiViewPool.TryGetValue(typeof(T).Name, out UIView value);

            return value as T;
        }

        private async UniTask<T> InitAsync<T>(Action<T> onCompleted = null, CancellationToken cancellationToken = default) where T : UIView
        {
            UIModelAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(UIModelAttribute)) as UIModelAttribute;

            if (attribute == null)
            {
                this.GetLogger().Error("[UIView]Need to apply UIModelAttribute on class {name}", typeof(T).Name);

                return null;
            }

            string modelName = attribute.ModelName;

            GameObject inst = null;
            string prefabPath = string.Empty;

            if (_addressableManager.IsInit && _addressableManager.IsReadyToGetBundle)
            {
                prefabPath = modelName;
                inst = await _addressableManager.InstantiateAsync(prefabPath,
                    parent: GetCanvasWithTag(UICanvasType.RootCanvas, attribute.SceneName), retryCount: 5,
                    cancellationToken: cancellationToken);
            }

            T view = inst != null ? inst.GetComponent<T>() : null;
            onCompleted?.Invoke(view);

            return view;
        }

        private void InitCompleted<T>(T instance, Transform root = null) where T : UIView
        {
            if (instance == null)
            {
                this.GetLogger().Error("[UIView]Null reference of type {type}", typeof(T).Name);

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
            
            Add(instance);
        }

        #endregion

        #region Canvas Handle
        
        private Dictionary<string, Transform> _uiCanvasPool = new Dictionary<string, Transform>();

        public Transform GetCanvasWithTag(UICanvasType enumTag)
        {
            string newTag = !(enumTag is UICanvasType.None) ? enumTag.ToString() : UICanvasType.RootCanvas.ToString();

            if (_uiCanvasPool.TryGetValue(newTag, out Transform value))
            {
                return value;
            }

            GameObject obj = GameObject.FindGameObjectWithTag(newTag);
            if (obj != null)
            {
                _uiCanvasPool.TryAdd(newTag, obj.transform);

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
            if (_uiCanvasPool.TryGetValue(key, out Transform result))
            {
                return result;
            }

            GameObject[] objects = GameObject.FindGameObjectsWithTag(newTag);
            for (int i = 0; i < objects.Length; ++i)
            {
                if (objects[i].scene.name.Equals(scene.name, StringComparison.CurrentCulture))
                {
                    Transform final = objects[i].transform.FindChildRecursive<Transform>(RootName);
                    _uiCanvasPool.TryAdd($"{newTag}-{scene.name}", final);

                    return final;
                }
            }
            
            this.GetLogger().Warn("Cannot find canvas with tag {tag} in scene: {sceneName}", newTag, sceneName);

            return null;
        }
        
        public void Remove(UICanvasType canvasType)
        {
            string key = canvasType.ToString();
            if (_uiCanvasPool.ContainsKey(key))
            {
                _uiCanvasPool.Remove(key);
            }
        }

        public void Remove(UICanvasType canvasType, string sceneName)
        {
            string key = $"{canvasType.ToString()}-{sceneName}";
            if (_uiCanvasPool.ContainsKey(key))
            {
                _uiCanvasPool.Remove(key);
            }
        }

        #endregion

        public void Init()
        {
            _addressableManager = ServiceLocator.GetService<AddressableManager>();
        }

        public void DeInit()
        {
            _uiCanvasPool.Clear();
            _uiViewPool.Clear();
        }
    }
}

