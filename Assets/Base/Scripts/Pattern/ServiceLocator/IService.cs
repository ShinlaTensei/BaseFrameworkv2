using System;
using System.Threading;

namespace Base.Pattern
{
    public interface IService : IDisposable
    {
        void Init();
        void DeInit();
    }

    public abstract class BaseService : IService
    {
        private CancellationTokenSource m_tokenSource;

        public CancellationTokenSource TokenSource => m_tokenSource;

        public virtual void Dispose()
        {
            if (m_tokenSource != null) m_tokenSource.Dispose();
        }

        public virtual void Init()
        {
            m_tokenSource = new CancellationTokenSource();
        }

        public virtual void DeInit()
        {
            m_tokenSource.Cancel();
        }
    }
    
    public abstract class BaseService<T> : IService<T>
    {
        public virtual void UpdateData(T data) {}

        public virtual void Dispose() {}

        public virtual void Init() {}

        public virtual void DeInit() {}
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

