using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestClass : MonoBehaviour
{
    private void Start()
    {
        UIModelAttribute attribute = typeof(Cube).GetCustomAttribute(typeof(UIModelAttribute)) as UIModelAttribute;
        Addressables.InstantiateAsync(attribute.ModelName, Vector3.zero, Quaternion.identity);
    }
}
