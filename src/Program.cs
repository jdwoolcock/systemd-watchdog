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
            //var (watchdogEnabled, resetInterval) = Tuple.Create(true, TimeSpan.FromSeconds(1));

            if (watchdogEnabled)
            {
                Console.WriteLine($"Watchdog Reset Interval {resetInterval}");
                Observable.Interval(resetInterval).Take(10).Subscribe(x =>
                {
                    Console.WriteLine("Keep Alive");
                    ServiceManager.KeepAlive();
                }, onCompleted: () => {Console.WriteLine("Failed");});
            }

            while (true)
            {
                Console.WriteLine("Hello Watchdog");
                Thread.Sleep(5000);
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