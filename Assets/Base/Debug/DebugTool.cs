using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using UnityEngine;

namespace Base
{
    public class DebugTool : BaseMono
    {
        [SerializeField] private FloatingButton floatingButton;
        [SerializeField] private GameObject debugContent;

        protected override void Start()
        {
            base.Start();

            floatingButton.OnClick += OpenDebugUI;
        }

        private void OnDestroy()
        {
            floatingButton.OnClick -= OpenDebugUI;
        }

        private void OpenDebugUI()
        {
            debugContent.SetActive(true);
            floatingButton.Active = false;
        }
    }
}

