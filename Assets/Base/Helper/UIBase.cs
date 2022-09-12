using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public abstract class UIBase : BaseMono
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void Next();
        public abstract void Back();

        public virtual void Show<T>(T argument) {}
        public virtual void Hide<T>(T argument) {}
    }
}

