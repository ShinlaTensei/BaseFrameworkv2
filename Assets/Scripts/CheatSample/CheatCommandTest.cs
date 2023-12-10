using System;
using System.Collections;
using System.Collections.Generic;
using Base.Cheat;
using Base.Helper;
using Base.Logging;
using UnityEngine;

public class CheatCommandTest : BaseMono
{
    [CheatCommand("Cheat Command One", category: "Sample")]
    private void CheatCommand_One(int one, bool two, string three)
    {
        PDebug.Info("Cheat Command 1");
    }
    
    [CheatCommand("CheatCommand Two", category: "Sample")]
    private void CheatCommand_Two(TestEnum testEnum, int one, float two)
    {
        PDebug.Info("Cheat Command 2");
    }

    private void Awake()
    {
        this.RegisterCallerInstance();
    }

    public enum TestEnum
    {
        One, Two, Three
    }
}

