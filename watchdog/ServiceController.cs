using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace watchdog
{
    public class ServiceController : IServiceWatchdog
    {
        private readonly Dictionary<Guid, DateTimeOffset> componentLastContact;
        private readonly ILogger logger;

        public ServiceController(ILoggerFactory loggerFactory, ISystemd systemd, IOptions<ServiceControllerOptions> options)
        {
            logger = loggerFactory.CreateLogger(GetType());
            var serviceOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            logger.LogDebug("Initialise ServiceController");

            componentLastContact = new Dictionary<Guid, DateTimeOffset>();
            try
            {
                var (watchdogEnabled, watchdogTimeout) = systemd.IsWatchdogEnabled();

                logger.LogDebug($"watchdog enabled {watchdogEnabled} timeout :{watchdogTimeout}");

                if (watchdogEnabled)
                {
                    // Recommended behaviour is to use half the interval returned by sd_watchdog_enabled
                    var resetInterval = TimeSpan.FromMilliseconds(watchdogTimeout.TotalMilliseconds / 2);

                    Observable.Interval(resetInterval).Subscribe(x =>
                    {
                        // Have all registered components made contact within the 
                        if (componentLastContact.Values.All(lastContact =>
                            DateTimeOffset.Now.Subtract(lastContact) < serviceOptions.StaleDuration))
                        {
                            try
                            {
                                logger.LogDebug($"Reset Watchdog every {resetInterval}");
                                systemd.ResetWatchdog();
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Failed to reset watchdog");
                            }
                        }
                        else
                        {
                            logger.LogWarning($"Watchdog not Reset as last contact > {serviceOptions.StaleDuration}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
               logger.LogError(e, "Failed to get watchdog status");
            }
        }

        public void KeepAlive(Guid id)
        {
            lock (this)
            {
                if (componentLastContact.ContainsKey(id))
                {
                    componentLastContact[id] = DateTimeOffset.Now;
                }
                else
                {
                    componentLastContact.Add(id, DateTimeOffset.Now);
                }
            }
        }
    }
}
