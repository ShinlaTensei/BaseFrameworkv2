using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Pattern;
using UnityEngine;

namespace Base
{
    public class InputHandler : BaseMono, IService
    {
        private InputAction inputAction;

        public InputAction InputAction => inputAction;

        public void CreateInputAction()
        {
            if (Application.platform is RuntimePlatform.Android)
            {
                inputAction = CacheGameObject.AddComponent<TouchInputAction>();
            }
            else if (Application.platform is RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor)
            {
                inputAction = CacheGameObject.AddComponent<MouseInputAction>();
            }
        }

        public void Init()
        {
            CreateInputAction();
        }
    }
}

