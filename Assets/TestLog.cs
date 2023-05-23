using System;
using Base;
using Base.Helper;
using Base.Logging;
using Base.Services;
using Base.Module;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLog : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text;
    [SerializeField, TextArea(5, 10)] private string m_testString;
    private CompositeDisposable _disposable = new CompositeDisposable();
    public void Start()
    {
        PDebug.DebugFormat("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
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
    }

    private void OnDisable()
    {
        PDebug.DebugFormat("[{0}] Time run: {1}", this.GetType(), DateTime.Now.ToFileTime());
        _disposable.Clear();
    }

    private void OnEnable()
    {
        var clickStream = Observable.EveryUpdate().Where(source => Input.GetMouseButtonDown(0));
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250f))).Where(source => source.Count >= 2)
            .Subscribe(source => UnloadScene()).AddTo(_disposable);

        Observable.EveryUpdate().Where(source => Input.GetKeyDown(KeyCode.Space)).Subscribe(source =>
        {
            PDebug.DebugFormat("ServiceLocator Instance: {0}", ServiceLocator.Instance);
        }).AddTo(_disposable);
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync("TestScene");
    }
    
    [NaughtyAttributes.Button("Test Get Size", EButtonEnableMode.Editor)]
    public void TestGetSize()
    {
        Vector2 size = UtilsClass.GetSizeOfText(m_text, m_text.fontSize, m_testString, 500f);
        m_text.text = m_testString;
        
        Debug.LogFormat("Test Get Size {0}", size);
    }
}

[Serializable]
public class TestData
{
    public int amount = 0;
    public string name = "Phong";
}

public enum TestEvent{OnNotify}
