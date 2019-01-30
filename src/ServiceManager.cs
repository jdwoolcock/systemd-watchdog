using System;
using System.Runtime.InteropServices;

namespace systemd
{
    class ServiceManager
    {
        /// <summary>
        /// Tells the service manager that service startup is finished,
        /// or the service finished loading its configuration.
        /// This is only used by systemd if the service definition file has Type=notify set.
        /// Since there is little value in signaling non-readiness, the only value services should send is "READY=1
        /// </summary>
        public static void NotifyReady()
        {
            try
            {
                Systemd.sd_notify(0, "READY=1");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Check whether the service manager expects watchdog keep-alive notifications from a service
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is watchdog enabled]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWatchdogEnabled()
        {
            try
            {
                // On failure, this call returns a negative errno - style error code.
                // If the service manager expects watchdog keep - alive notification messages to be sent,
                // > 0 is returned, otherwise 0 is returned.
                // Only if the return value is > 0, the usec parameter is valid after the call.

                var result = Systemd.sd_watchdog_enabled(0, out var seconds);
                Console.WriteLine($"Watchdog enabled {result} seconds {seconds}");
                return result > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
