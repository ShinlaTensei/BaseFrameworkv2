using Base;
using Base.Logging;
using Base.MessageSystem;
using Base.Pattern;
using UnityEngine;

public class TestClass : MonoBehaviour
{
    private void Start()
    {
        OnTest(null);
    }

    private void OnTest(object argument)
    {
        ServiceLocator.GetService<InputHandler>().Init();
        ServiceLocator.GetService<ObserverManager>().Init();
        ServiceLocator.GetService<AddressableManager>().Init();
        
        this.GetLogger().Info("Test log info");
        this.GetLogger().Debug("Test log debug");
        this.GetLogger().Warn("Test log warning");
        this.GetLogger().Error("Test log error");
    }
}
