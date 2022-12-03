using System;
using System.Reflection;
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
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        ServiceLocator.Get<Cube>().Hide();
    }
}
