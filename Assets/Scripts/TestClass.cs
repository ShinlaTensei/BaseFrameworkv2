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
        Test().Forget();
    }

    private async UniTaskVoid Test()
    {
        var attribute = typeof(Cube).GetCustomAttribute(typeof(UIModelAttribute)) as UIModelAttribute;
        var handle = Addressables.InstantiateAsync(attribute.ModelName, Vector3.zero, Quaternion.identity);
        await handle.Task;
        
        ServiceLocator.Set(handle.Result.GetComponent<Cube>());
        var cube = ServiceLocator.Get<Cube>();
        cube.RegisterListener(Event1.Lalala, OnTest);
        this.PostEvent(Event1.Lalala, null);
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        cube.RemoveListener(Event1.Lalala, OnTest);
    }

    private void OnTest(object argument)
    {
        this.GetLogger().Info("Test event with ServiceLocator");
    }
}

public enum Event1 {Lalala}
