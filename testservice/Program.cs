using System;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using watchdog;

namespace testservice
{
    class Program
    {

        public static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

            // Configuration providers
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // createSerilog logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            // DI setup
            var services = new ServiceCollection()
                // Logging
                .AddLogging(builder =>
                {
                    builder
                        .AddSerilog(dispose: true);
                })
                // Configuration via options
                .AddOptions()
                .Configure<TestServiceOptions>("Service1", config.GetSection("Service1"))
                .Configure<TestServiceOptions>("Service2", config.GetSection("Service2"))
                .Configure<ServiceControllerOptions>(config.GetSection("ServiceControllerOptions"))

                // Register the service and other dependencies as required
                .AddSingleton<ISystemd, StubSystemd>()
                .AddSingleton<IServiceWatchdog, ServiceController>()
                // ... and finally build the service provider
                .BuildServiceProvider();


            var service1 = new TestService(services.GetRequiredService<ILoggerFactory>(), 
                                          services.GetRequiredService<IServiceWatchdog>(),
                                          services.GetRequiredService<IOptionsMonitor<TestServiceOptions>>().Get("Service1"));
            var service2 = new TestService(services.GetRequiredService<ILoggerFactory>(),
                services.GetRequiredService<IServiceWatchdog>(),
                services.GetRequiredService<IOptionsMonitor<TestServiceOptions>>().Get("Service2"));


            while (true) // Loop indefinitely
            {
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