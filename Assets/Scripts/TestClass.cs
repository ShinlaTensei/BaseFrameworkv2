using System;
using System.Reflection;
using Base.Logging;
using Base.MessageSystem;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestClass : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void OnTest(object argument)
    {
        this.GetLogger().Info("Test event with ServiceLocator");
    }
}

public enum Event1 {Lalala}
