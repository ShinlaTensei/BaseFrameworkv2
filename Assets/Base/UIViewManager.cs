using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
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
    public class UIViewManager : BaseMono, IService
    {
        private const string RootName = "Root";

        #region UIView Handle
        
        private Dictionary<string, UIView> _uiViewPool = new Dictionary<string, UIView>();
        
        private UIView _previous;
        private UIView _current;

        private AddressableManager _addressableManager;

        private async UniTask<T> ShowAsync<T>(T instance, Action onInit = null, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T view = instance ? instance : TryGetView<T>();

            if (!view)
            {
                view = await InitAsync<T>(null, cancellationToken);
            }

            if (view)
            {
                InitCompleted(view, root);
            }
            
            onInit?.Invoke();

            return view;
        }

        public async UniTaskVoid Show<T>(Action onInit = null, Transform root = null, CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                _current.Show();
                if (_previous && _current.ClosePrevOnShow) _previous.Hide();
            }
        }

        public async UniTaskVoid Show<T, Y>(Y value, Action onInit = null, Transform root = null,
            CancellationToken cancellationToken = default) where T : UIView
        {
            T inst = await ShowAsync<T>(null, onInit, root, cancellationToken).AttachExternalCancellation(cancellationToken);
            if (inst)
            {
                _previous = _current;
                _current = inst;
                await _current.Await(cancellationToken).AttachExternalCancellation(cancellationToken);
                _current.Show(value);
                if (_previous && _current.ClosePrevOnShow) _previous.Hide();
            }
        }
        
        public void Add<T>(T view) where T : UIView
        {
            if (!_uiViewPool.ContainsKey(typeof(T).Name))
            {
                _uiViewPool.TryAdd(typeof(T).Name, view);
            }
        }

        public void Remove(UIView value)
        {
            string key = _uiViewPool.FirstOrDefault(item => item.Value.Equals(value)).Key;
            _uiViewPool.Remove(key);
        }
        
        private T TryGetView<T>() where T : UIView
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
                inst = await _addressableManager.InstantiateAsync(prefabPath, retryCount: 5, cancellationToken: cancellationToken);
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
            instance.CacheTransform.SetAsLastSibling();
            instance.Active = instance.ActiveDefault;
            
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
            
            this.GetLogger().Debug("Cannot find canvas with tag {tag} in scene: {sceneName}", newTag, sceneName);

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
            _uiViewPool.Clear();
            _uiCanvasPool.Clear();

            _addressableManager = ServiceLocator.Get<AddressableManager>();
        }
    }
}

