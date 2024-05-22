using System;
using Base;
using Base.Helper;
using Base.Logging;
using Base.Module;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLog : MonoBehaviour
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    public void Start()
    {
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
        _disposable.Clear();
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
