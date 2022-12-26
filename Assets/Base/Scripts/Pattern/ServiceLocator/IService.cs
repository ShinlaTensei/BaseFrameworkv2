using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Pattern
{
    public interface IService
    {
        void Init();
        void DeInit();
    }

    public interface IService<T> : IService
    {
        void UpdateData(T data);
    }
}

