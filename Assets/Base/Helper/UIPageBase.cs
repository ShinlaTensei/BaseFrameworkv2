using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Helper
{
    public abstract class UIPageBase : BaseMono
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void Next();
        public abstract void Back();

        public virtual void Show<T>() {}
        public virtual void Hide<T>() {}
    }
}

