using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Module;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    public void Start()
    {
        BaseLogManager.BaseLogger.Info("Test log Info v3");
        BaseLogManager.BaseLogger.Debug("Test log");
    }
}
