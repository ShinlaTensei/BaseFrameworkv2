using System;
using System.Threading;
using Base.Logging;
using UniRx;
using UnityEngine;

namespace Base.Helper
{
    public class BaseInterval : SingletonNonMono<BaseInterval>
    {
        private CompositeDisposable m_compositeDisposable;

        private CompositeDisposable CompositeDisposable => LazyInitializer.EnsureInitialized(ref m_compositeDisposable);

        public static void RunInterval(float timeInSeconds, Action callBack = null)
        {
            IDisposable disposable = Observable.Interval(TimeSpan.FromSeconds(timeInSeconds)).Subscribe(_ =>
                                                    {
                                                        callBack?.Invoke();
                                                    }, Instance.OnError);
            
            Instance.CompositeDisposable.Add(disposable);
        }

        public static void RunInterval<T>(float timeInSeconds, T data, Action<T> callback = null)
        {
            IDisposable disposable = Observable.Interval(TimeSpan.FromSeconds(timeInSeconds), Scheduler.MainThread).Subscribe(_ =>
                                                                             {
                                                                                 callback?.Invoke(data);
                                                                             }, Instance.OnError);
            
            Instance.CompositeDisposable.Add(disposable);
        }

        public static void RunAfterTime(float timeInSeconds, IScheduler scheduler = null, Action startAction = null, Action onComplete = null)
        {
            if (scheduler is null) scheduler = Scheduler.MainThread;

            IDisposable disposable = Observable.Start(startAction, TimeSpan.FromSeconds(timeInSeconds), scheduler)
                      .Subscribe(_ => onComplete?.Invoke(), onError: Instance.OnError);
            
            Instance.CompositeDisposable.Add(disposable);
        }

        private void OnError(Exception exception)
        {
            PDebug.Error(exception, "[BaseInterval] Exception ERROR: {0}", exception.Message);
        }

        public void Cancel()
        {
            CompositeDisposable.Clear();
        }
    }
}

