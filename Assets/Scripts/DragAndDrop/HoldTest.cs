#region Header
// Date: 28/05/2023
// Created by: Huynh Phong Tran
// File name: HoldTest.cs
#endregion

using System;
using Base.Helper;
using Base.Logging;
using UnityEngine;

public class HoldTest : BaseMono
{
    [SerializeField] private HoldDetector m_holdDetector;

    protected override void Start()
    {
        //m_holdDetector.HoldEvent.Subscribe(OnHoldOnce, true);
    }

    private void OnHoldOnce(bool isHold)
    {
        PDebug.Info("Hold event handle");
    }
}