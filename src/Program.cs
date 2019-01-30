using System;
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
            while (true)
            {

                ServiceManager.IsWatchdogEnabled();
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