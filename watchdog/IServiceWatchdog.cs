using System;

namespace watchdog
{
    public interface IServiceWatchdog
    {
        void KeepAlive(Guid id);
    }
}
