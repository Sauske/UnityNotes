using System;

namespace UMI
{
    public interface IModule : IDisposable
    {
        void FreeMemory();
    }

    public interface IUpdate
    {
        void OnUpdateEx(float fDeltaTime);
    }
}


