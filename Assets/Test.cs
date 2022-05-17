using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using UnityEngine;

public class Test : BaseMono
{
    public void Start()
    {
        BaseInterval.RunAtEveryFrame(this, 1f, 1f, () =>
        {
            Debug.Log("Test");
        });
    }
}
