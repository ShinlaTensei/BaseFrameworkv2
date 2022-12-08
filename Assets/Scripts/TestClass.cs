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
        ServiceLocator.Get<InputHandler>().Init();
        ServiceLocator.Get<ObserverManager>().Init();
    }
}
