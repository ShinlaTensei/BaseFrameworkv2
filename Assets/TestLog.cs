using System;
using Base.Helper;
using Base.Logging;
using Base.MessageSystem;
using Base.Module;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    public void Start()
    {
        this.GetLogger().Info("Test log Info v3");
        this.GetLogger().Info("Test log");

        string message = "I am a God";
        string encrypted = Encryption.Encrypt(message);
        this.GetLogger().Info("{msg}", encrypted);
        string decrypted = Encryption.Decrypt(encrypted);
        this.GetLogger().Info("{msg}", decrypted);
        // TestData test = new TestData {name = "LALLALALA", amount = 1000};
        // FileUtilities.SaveDataWithEncrypted(FileUtilities.GetSystemPath(),"TestEncrypt.bin", test);

        var data = FileUtilities.LoadDataWithEncrypted<TestData>(FileUtilities.GetSystemPath() + "/TestEncrypt.bin");
        this.GetLogger().Info("Load Data: {@data}", data);
    }

    private void OnEnable()
    {
        this.RegisterListener<object>(TestEvent.OnNotify, ResponseNotifyEvent);
        
        this.PostEvent<object>(TestEvent.OnNotify, null);
    }

    private void ResponseNotifyEvent(object argument)
    {
        this.GetLogger().Info("Event called successfully");
    }
}

[Serializable]
public class TestData
{
    public int amount = 0;
    public string name = "Phong";
}

public enum TestEvent{OnNotify}
