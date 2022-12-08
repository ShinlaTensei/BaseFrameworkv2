using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Base
{
    public class AddressableManager : MonoBehaviour, IService
    {
        public static float RETRY_DELAY_TIMER = 2f;
        public bool IsInit { get; set; }
        public bool IsReadyToGetBundle { get; set; }

        #region Init & Update

        public void Init()
        {
            
        }

        public void Initialize(Action<bool> callback, int retryCount = 0, int retry = 0)
        {
            BaseLogSystem.GetLogger().Info("[AddressableManager] Initializing ...");

            IsInit = false;
            IsReadyToGetBundle = false;

            try
            {
                Addressables.InitializeAsync().Completed += OnCompleted;
            }
            catch (Exception e)
            {
                Retry(e);
            }

            void OnCompleted(AsyncOperationHandle<IResourceLocator> handle)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    CallOnMainThread();
                }
                else if (handle.Status == AsyncOperationStatus.Failed)
                {
                    Retry(handle.OperationException);
                }
            }

            void CallOnMainThread()
            {
                BaseLogSystem.GetLogger().Info("[AddressableManager] Initializing Completed!!!");

                IsInit = true;
                IsReadyToGetBundle = true;
                callback?.Invoke(true);
            }

            void Retry(Exception ex)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager] Initializing Error: {msg}", ex.Message);
                    IsInit             = true;
                    IsReadyToGetBundle = false;
                    callback?.Invoke(false);
                }
                else
                {
                    retry++;
                    void CallRetry()
                    {
                        BaseLogSystem.GetLogger().Info("[AddressableManager]Initialized retry");
                        Initialize(callback, retryCount, retry);
                    }
                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }

        public async UniTask<bool> InitializeAsync(Action<bool> callback, int retryCount = 0, int retry = 0,
            CancellationToken cancellationToken = default)
        {
            BaseLogSystem.GetLogger().Info("[AddressableManager] Initializing ...");

            IsInit = false;
            IsReadyToGetBundle = false;

            try
            {
                await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);

                BaseLogSystem.GetLogger().Info("[AddressableManager] Initializing Completed!!!");
                IsInit = IsReadyToGetBundle = true;

                return true;
            }
            catch (OperationCanceledException canceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager] Initializing Error: {msg}", exception.Message);
                    IsInit             = true;
                    IsReadyToGetBundle = false;
                    callback?.Invoke(false);

                    return false;
                }
                else
                {
                    retry++;
                    BaseLogSystem.GetLogger().Info("[AddressableManager] Initializing Retry");

                    return await InitializeAsync(callback, retryCount, retry, cancellationToken: cancellationToken);
                }
            }
        }

        #endregion

        public static void Dispatch(float delay, Action action)
        {
            UnityMainThreadDispatcher.Instance().Dispatch(delay, action, WaitForNetwork);
        }
        static IEnumerator WaitForNetwork()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            }
        }

        #region LoadAssset

        public AsyncOperationHandle<T> LoadAsset<T>(object key)
        {
            return Addressables.LoadAssetAsync<T>(key);
        }

        public async UniTask<T> LoadAsset<T>(object key, int retryCount = 0, int retry = 0, CancellationToken cancellationToken = default) where T : Object
        {
            try
            {
                return await Addressables.LoadAssetAsync<T>(key).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager] LoadAsset {asset} Error {error}", key, exception.Message);

                    return null;
                }
                else
                {
                    retry++;
                    await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), ignoreTimeScale: true, cancellationToken: cancellationToken);
                    BaseLogSystem.GetLogger().Error("[AddressableManager] LoadAsset {asset} Retry ... {count}", key, retry);

                    return await LoadAsset<T>(key, retryCount, retry, cancellationToken: cancellationToken);
                }
            }
        }

        public async UniTask<GameObject> InstantiateAsync(object key,Transform parent = null, bool instantiateInWorld = false, int retryCount = 0, 
            int retry = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.InstantiateAsync(key, parent, instantiateInWorld).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager] Instantiate Object {obj} Error {error}", key, exception.Message);
                    
                    return null;
                }

                retry++;
                await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), true, cancellationToken: cancellationToken);
                BaseLogSystem.GetLogger().Info("[AddressableManager Retry Count {count}", retry);

                return await InstantiateAsync(key, parent, instantiateInWorld, retryCount, retry, cancellationToken);
            }
        }

        #endregion

        #region LoadScene

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, Action<SceneInstance> callback = null,
            LoadSceneMode mode = LoadSceneMode.Additive, bool activeOnLoad = false, int retryCount = 0, int retry = 0)
        {
            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, mode, activeOnLoad);
                handle.Completed += OnCompleted;

                return handle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }

            void OnCompleted(AsyncOperationHandle<SceneInstance> argument)
            {
                if (argument.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(argument.Result);
                }
                else
                {
                    CheckThenRetry(argument, argument.OperationException);
                }
            }
            
            void CheckThenRetry(AsyncOperationHandle<SceneInstance> result, Exception e)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager]LoadSceneAsync '{key}'. Error: '{error}'", key, e.Message);
                    callback?.Invoke(result.Result);
                }
                else
                {
                    retry++;
                    void CallRetry()
                    {
                        BaseLogSystem.GetLogger().Info("[AddressableManager]LoadSceneAsync retry '{key}'", key);
                        LoadSceneAsync(key, callback, mode, activeOnLoad, retryCount, retry);
                    }
                    Dispatch(action: CallRetry, delay: RETRY_DELAY_TIMER);
                }
            }
        }

        public async UniTask<SceneInstance?> LoadSceneAsync(object key, LoadSceneMode mode = LoadSceneMode.Additive, bool activeOnLoad = false,
            int retryCount = 0, int retry = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.LoadSceneAsync(key, mode, activeOnLoad).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException canceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                if (retry >= retryCount)
                {
                    BaseLogSystem.GetLogger().Error("[AddressableManager]LoadSceneAsync '{key}'. Error: '{error}'", key, exception.Message);
                    return null;
                }
                else
                {
                    retry++;
                    await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIMER), ignoreTimeScale: true, cancellationToken: cancellationToken);
                    BaseLogSystem.GetLogger().Info("[AddressableManager]LoadSceneAsync retry {0} times", retry);
                    return await LoadSceneAsync(key, mode, activeOnLoad, retryCount, retry, cancellationToken);
                }
            }
        }
        
        public static AsyncOperationHandle<SceneInstance>? UnloadScene(SceneInstance scene, Action<bool> callback, bool autoReleaseHandle = true)
        {
            void OnComplete(AsyncOperationHandle<SceneInstance> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread();
                }
                else
                {
                    CheckThenRetry(result.OperationException);
                }
            }

            void CallInMainThread()
            {
                callback?.Invoke(true);
            }
            void CheckThenRetry(Exception e)
            {
                BaseLogSystem.GetLogger().Error("[AddressableManager]UnloadScene Error: '{error}'", e.Message);
                callback?.Invoke(false);
            }

            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(scene, autoReleaseHandle);
                handle.Completed += OnComplete;
                return handle;
            }
            catch (Exception e)
            {
                CheckThenRetry(e);
            }
            return null;
        }
        public static async UniTask<SceneInstance?> UnloadScene(SceneInstance scene, bool autoReleaseHandle = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.UnloadSceneAsync(scene, autoReleaseHandle).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return null;
            }
            catch (Exception e)
            {
                BaseLogSystem.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);
                return null;
            }
        }

        public static AsyncOperationHandle<SceneInstance>? UnloadScene(AsyncOperationHandle handle, Action<bool> callback, bool autoReleaseHandle = true)
        {
            void OnComplete(AsyncOperationHandle<SceneInstance> result)
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    CallInMainThread();
                }
                else
                {
                    CheckThenRetry(result.OperationException);
                }
            }

            void CallInMainThread()
            {
                callback?.Invoke(true);
            }
            void CheckThenRetry(Exception e)
            {
                BaseLogSystem.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);
                callback?.Invoke(false);
            }

            try
            {
                AsyncOperationHandle<SceneInstance> ops = Addressables.UnloadSceneAsync(handle, autoReleaseHandle);
                ops.Completed += OnComplete;
                return ops;
            }
            catch (Exception e)
            {
                CheckThenRetry(e);
            }
            return null;
        }
        public static async UniTask<SceneInstance?> UnloadScene(AsyncOperationHandle handle, bool autoReleaseHandle = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Addressables.UnloadSceneAsync(handle, autoReleaseHandle).ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return null;
            }
            catch (Exception e)
            {
                BaseLogSystem.GetLogger().Error("[AddressableManager]UnloadScene Error: '{e}'", e.Message);
                return null;
            }
        }

        public static AsyncOperationHandle<SceneInstance>? UnloadScene(AsyncOperationHandle? handle, Action<bool> callback, bool autoReleaseHandle = true)
        {
            if (handle != null && handle.HasValue)
            {
                return UnloadScene(handle.Value, callback, autoReleaseHandle);
            }
            callback?.Invoke(true);
            return null;
        }
        public static async UniTask<SceneInstance?> UnloadScene(AsyncOperationHandle? handle, bool autoReleaseHandle = true, CancellationToken cancellationToken = default)
        {
            if (handle != null && handle.HasValue)
            {
                return await UnloadScene(handle.Value, autoReleaseHandle, cancellationToken);
            }
            return null;
        }

        #endregion
    }
}

