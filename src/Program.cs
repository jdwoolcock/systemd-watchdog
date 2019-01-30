using System;
using System.Reactive.Linq;
using System.Runtime.Loader;
using System.Threading;

namespace systemd
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

            Thread.Sleep(2000);
            ServiceManager.NotifyReady();

            var(watchdogEnabled, resetInterval) = ServiceManager.IsWatchdogEnabled();

            if (watchdogEnabled)
            {
                Console.WriteLine($"Watchdog Reset Period {resetInterval}");
                Observable.Interval(resetInterval).Take(5).Subscribe(x =>
                {
                    Console.WriteLine("Keep Alive");
                    ServiceManager.KeepAlive();
                }, onCompleted: () => {Console.WriteLine("Something Went wrong!!");});
            }

            while (true)
            {
                Console.WriteLine("Hello World!");
                Thread.Sleep(2000);
            }
        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            System.Console.WriteLine("Unloading...");
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            System.Console.WriteLine("Exiting...");
        }
    }
}