using System;
using Base;
using Base.Helper;
using Base.Logging;
using Base.Services;
using Base.Module;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLog : MonoBehaviour
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    public void Start()
    {
        this.GetLogger().Debug("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
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

    private void OnDisable()
    {
        this.GetLogger().Debug("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
        _disposable.Clear();
    }

    private void OnEnable()
    {
        var clickStream = Observable.EveryUpdate().Where(source => Input.GetMouseButtonDown(0));
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250f))).Where(source => source.Count >= 2)
            .Subscribe(source => UnloadScene()).AddTo(_disposable);

        Observable.EveryUpdate().Where(source => Input.GetKeyDown(KeyCode.Space)).Subscribe(source =>
        {
            this.GetLogger().Debug("ServiceLocator Instance: {0}", ServiceLocator.Instance);
        }).AddTo(_disposable);
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync("TestScene");
    }
}

[Serializable]
public class TestData
{
    public int amount = 0;
    public string name = "Phong";
}

public enum TestEvent{OnNotify}
