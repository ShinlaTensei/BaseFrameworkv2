using System;
using Base;
using Base.Core;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Sirenix.Serialization;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class TestLog : BaseMono
{
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioDataContainer m_audioDataContainer;
    [SerializeField] private UnityEvent m_event;

    protected override void Start()
    {
        PoolSystem.CreatePool(m_audioSource);
        ServiceLocator.Get<AudioService>().Init();
        ServiceLocator.Get<AudioService>().UpdateData(m_audioDataContainer);
        BaseInterval.RunInterval(1f, m_event.Invoke);
        
        PDebug.DebugFormat("[{0}] Time run: {1}", this.GetType(), DateTime.Now);
        PDebug.Info("Test log Info v3");
        PDebug.Info("Test log");

        string message = "I am a God";
        string encrypted = Encryption.Encrypt(message);
        PDebug.InfoFormat("{msg}", encrypted);
        string decrypted = Encryption.Decrypt(encrypted);
        PDebug.InfoFormat("{msg}", decrypted);
        // TestData test = new TestData {name = "LALLALALA", amount = 1000};
        // FileUtilities.SaveDataWithEncrypted(FileUtilities.GetSystemPath(),"TestEncrypt.bin", test);

        var data = FileUtilities.LoadDataWithEncrypted<TestData>(FileUtilities.GetSystemPath() + "/TestEncrypt.bin");
        PDebug.InfoFormat("Load Data: {@data}", data);
        PDebug.Trace("Test trace");
        PDebug.Debug("Test Debug");
        PDebug.Info("Test info");
        PDebug.Warn("Test warning");
        PDebug.Error("Test error");
    }

    private void OnDisable()
    {
        PDebug.DebugFormat("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
    }

    private void OnDestroy()
    {
        PDebug.Shutdown();
    }
}

[Serializable]
public class TestData
{
    public int amount = 0;
    public string name = "Phong";
}

public enum TestEvent{OnNotify}
