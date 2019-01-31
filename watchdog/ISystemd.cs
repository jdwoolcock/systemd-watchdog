using System;

namespace watchdog
{
    public interface ISystemd 
    {
        /// <summary>
        /// Tells the service manager that service startup is finished,
        /// or the service finished loading its configuration.
        /// This is only used by systemd if the service definition file has Type=notify set.
        /// Since there is little value in signaling non-readiness, the only value services should send is "READY=1
        /// </summary>
        void NotifyReady();

        /// <summary>
        /// Check whether the service manager expects watchdog keep-alive notifications from a service
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is watchdog enabled]; otherwise, <c>false</c>.
        /// </returns>
        Tuple<bool, TimeSpan> IsWatchdogEnabled();

        /// <summary>
        /// Tells the service manager to update the watchdog timestamp.
        /// This is the keep-alive ping that services need to issue in regular intervals if WatchdogSec= is enabled for it. 
        /// </summary>
        void ResetWatchdog();
    }
}
