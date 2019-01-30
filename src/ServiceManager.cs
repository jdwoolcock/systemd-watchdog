using System;
using System.Runtime.InteropServices;

namespace systemd
{
    public class ServiceManager
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
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Systemd.sd_notify(0, "READY=1");
                }

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
        public static Tuple<bool, TimeSpan> IsWatchdogEnabled()
        {
            try
            {
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // On failure, this call returns a negative errno - style error code.
                    // If the service manager expects watchdog keep - alive notification messages to be sent,
                    // > 0 is returned, otherwise 0 is returned.
                    // Only if the return value is > 0, the usec parameter is valid after the call.

                    var result = Systemd.sd_watchdog_enabled(0, out var microSeconds);
                
                    if (result > 0)
                    {
                        return Tuple.Create(true, TimeSpan.FromMilliseconds((microSeconds / 1000) / 2));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Tuple.Create(false, TimeSpan.Zero);
        }

        /// <summary>
        /// Tells the service manager to update the watchdog timestamp.
        /// This is the keep-alive ping that services need to issue in regular intervals if WatchdogSec= is enabled for it. 
        /// </summary>
        public static void KeepAlive()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Systemd.sd_notify(0, "WATCHDOG=1");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
