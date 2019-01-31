using System;
using watchdog;

namespace testservice
{
    public class StubSystemd : ISystemd
    {
        public void NotifyReady()
        {
           
        }

        public Tuple<bool, TimeSpan> IsWatchdogEnabled()
        {
            return Tuple.Create(true, TimeSpan.FromSeconds(5));
        }

        public void ResetWatchdog()
        {
        }
    }
}
