using System;
using System.Runtime.InteropServices;

namespace systemd
{
    public static class Systemd
    {
        [DllImport("libsystemd.so.0")]
        public static extern int sd_notify(int unset_environment, string state);

        [DllImport("libsystemd.so.0")]
        public static extern int sd_watchdog_enabled(int unset_environment, out ulong usec);
    }
}