using System;
using Base;
using Base.Helper;
using Base.Logging;
using Base.Services;
using Base.Pattern;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSignal : Signal<object>
{
    
}
public class TestClass : BaseMono
{
    private void OnEnable()
    {
        ServiceLocator.SetService<InputHandler>().Init();
        ServiceLocator.SetService<AddressableManager>().Init();
        ServiceLocator.SetSignal<TestSignal>().Subscribe(OnTest);
        ServiceLocator.SetSignal<TestSignal>().Dispatch(null);

        SceneManager.LoadScene("TestScene", LoadSceneMode.Additive);
    }

    private void OnApplicationQuit()
    {
        this.GetLogger().Debug("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
        ServiceLocator.GetService<AddressableManager>()?.DeInit();
        ServiceLocator.GetSignal<TestSignal>()?.UnSubscribe(OnTest);
        
        ServiceLocator.RemoveService<AddressableManager>();
        ServiceLocator.RemoveSignal<TestSignal>();
    }

    private void OnTest(object argument)
    {
        this.GetLogger().Info("Test log info");
        this.GetLogger().Debug("Test log debug");
        this.GetLogger().Warn("Test log warning");
        this.GetLogger().Error("Test log error");
    }
}
