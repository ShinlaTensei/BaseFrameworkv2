using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Base.Module;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    public void Start()
    {
        BaseLogManager.BaseLogger.Info("Test log Info v3");
        BaseLogManager.BaseLogger.Debug("Test log");

        string message = "I am a God";
        string encrypted = Encryption.Encrypt(message);
        BaseLogManager.BaseLogger.Info("{msg}", encrypted);
        string decrypted = Encryption.Decrypt(encrypted);
        BaseLogManager.BaseLogger.Info("{msg}", decrypted);
        // TestData test = new TestData {name = "LALLALALA", amount = 1000};
        // FileUtilities.SaveDataWithEncrypted(FileUtilities.GetSystemPath(),"TestEncrypt.bin", test);

        var data = FileUtilities.LoadDataWithEncrypted<TestData>(FileUtilities.GetSystemPath() + "/TestEncrypt.bin");
        BaseLogManager.BaseLogger.Info("Load Data: {@data}", data);
    }
}

[Serializable]
public class TestData
{
    public int amount = 0;
    public string name = "Phong";
}
