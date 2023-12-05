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
    private void CheatCommand_One()
    {
        PDebug.Info("Cheat Command 1");
    }

    private void Awake()
    {
        PDebug.Info("Cheat Command 1");
    }
}

