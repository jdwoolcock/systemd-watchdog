﻿using System;
using System.Runtime.InteropServices;

namespace watchdog
{
    public class Systemd : ISystemd
    {
        [DllImport("libsystemd.so.0")]
        public static extern int sd_notify(int unset_environment, string state);

        [DllImport("libsystemd.so.0")]
        public static extern int sd_watchdog_enabled(int unset_environment, out long usec);


        /// <summary>
        /// Tells the service manager that service startup is finished,
        /// or the service finished loading its configuration.
        /// This is only used by systemd if the service definition file has Type=notify set.
        /// Since there is little value in signaling non-readiness, the only value services should send is "READY=1
        /// </summary>
        public void NotifyReady()
        {
           sd_notify(0, "READY=1");
        }

        /// <summary>
        /// Check whether the service manager expects watchdog keep-alive notifications from a service
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is watchdog enabled]; otherwise, <c>false</c>.
        /// </returns>
        public Tuple<bool, TimeSpan> IsWatchdogEnabled()
        {

            // On failure, this call returns a negative errno - style error code.
            // If the service manager expects watchdog keep - alive notification messages to be sent,
            // > 0 is returned, otherwise 0 is returned.
            // Only if the return value is > 0, the usec parameter is valid after the call.
            var result = Systemd.sd_watchdog_enabled(0, out var microSeconds);

            return result > 0
                ? Tuple.Create(true, TimeSpan.FromMilliseconds(microSeconds / 1000d))
                : Tuple.Create(false, TimeSpan.Zero);
        }

        /// <summary>
        /// Tells the service manager to update the watchdog timestamp.
        /// This is the keep-alive ping that services need to issue in regular intervals if WatchdogSec= is enabled for it. 
        /// </summary>
        public void ResetWatchdog()
        {
            sd_notify(0, "WATCHDOG=1");
        }
    }
}