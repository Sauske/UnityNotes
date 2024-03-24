using System;

namespace SFramework
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


