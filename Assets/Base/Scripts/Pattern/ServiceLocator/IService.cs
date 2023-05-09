using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Pattern
{
    public interface IService : IDisposable
    {
        void Init();
        void DeInit();
    }

    public interface IService<T> : IService
    {
        void UpdateData(T data);
    }

    public interface IServiceUpdate : IDisposable
    {
        void Update();
    }
}

