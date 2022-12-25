using System;
using Base;
using Base.Logging;
using Base.MessageSystem;
using Base.Pattern;
using UniRx;
using UnityEngine;

public class TestSignal : Signal<object>
{
    
}
public class TestClass : MonoBehaviour
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    
    private void Start()
    {
        Observable.EveryUpdate().Where(source => Input.GetMouseButtonDown(0)).Subscribe(source =>
        {
            this.GetLogger().Debug("Mouse Click");
        }).AddTo(_disposable);
        
        ServiceLocator.GetSignal<TestSignal>().Subscribe(OnTest);
        ServiceLocator.GetSignal<TestSignal>().Dispatch(null);
    }

    private void OnDestroy()
    {
        _disposable.Clear();
        ServiceLocator.GetService<AddressableManager>().Dispose();
        ServiceLocator.GetSignal<TestSignal>().UnSubscribe(OnTest);
    }

    private void OnTest(object argument)
    {
        ServiceLocator.GetService<InputHandler>().Init();
        ServiceLocator.GetService<AddressableManager>().Init();
        
        this.GetLogger().Info("Test log info");
        this.GetLogger().Debug("Test log debug");
        this.GetLogger().Warn("Test log warning");
        this.GetLogger().Error("Test log error");
    }
}
