using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using watchdog;

namespace testservice
{
    public class TestService
    {
        private readonly ILogger logger;

        public TestService(ILoggerFactory loggerFactory, IServiceWatchdog serviceWatchdog, TestServiceOptions options)
        {
            this.logger = loggerFactory.CreateLogger(GetType());
            Guid id = Guid.NewGuid();
            

            Observable.Interval(options.Interval).Take(options.DieCount).Subscribe(x =>
            {
                logger.LogInformation($"{options.Name} is Alive! {x}");
                serviceWatchdog.KeepAlive(id);
            }, onCompleted: () => { logger.LogError($"{options.Name} Oh Snap!"); });
        }
    }
}
