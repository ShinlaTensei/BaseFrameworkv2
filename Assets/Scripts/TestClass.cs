using Base;
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
        ServiceLocator.Get<InputHandler>().CreateInputAction();
    }
}

public enum Event1 {Lalala}
