using AutoResultAmendment.Services;
using Fanex.Caching;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Transport;
using SharpRaven;

namespace AutoResultAmendmentService;

public class AutoService : IHostedService
{
    private readonly IMaintenanceService maintenanceService;

    private readonly IConfiguration configuration;

    private bool running;

    private int counter;

    public AutoService(
        IConfiguration configuration,
        IMaintenanceService maintenanceService)
    {
        this.configuration = configuration;
        this.maintenanceService = maintenanceService;
        InitializeExtraComponent();
        InitializeSetting();
    }

    private void InitializeSetting()
    {
        StartUmSetting();
        StartLogSetting();
    }

    private static void InitializeExtraComponent()
    {
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
    }

    private static void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        e.Exception.Handle(x =>
        {
            LogService.Error(x.Message, x);
            return true;
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        running = true;
        var autoRefundThread = new Thread(ThreadProcess);
        Console.WriteLine($"AutoResultAmendmentService Starting at {DateTime.Now}");
        autoRefundThread.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        running = false;
        Console.WriteLine($"AutoResultAmendmentService Stopping at {DateTime.Now}");
        return Task.CompletedTask;
    }

    private void ThreadProcess()
    {
        //while (running)
        //{
        if (!maintenanceService.IsMaintenance)
        {
            MainProcess();
            //var value = configuration.GetSection("ServiceInterval").Value;
            //if (value != null)
            //{
            //    Thread.Sleep(int.Parse(value));
            //}
        }
        //}
    }

    private void MainProcess()
    {
        try
        {
            //autoPayoutController.ProcessAsync();
            HandleGarbageCollector();
        }
        catch (Exception exception)
        {
            LogService.Error("Fanex.AutoResultAmendmentService: ", exception);
        }
    }

    private void HandleGarbageCollector()
    {
        var value = configuration.GetSection("GarbageCollectorInterval").Value;
        if (value != null && counter >= int.Parse(value))
        {
            counter = 0;
            GC.WaitForPendingFinalizers();
        }

        var interval = configuration.GetSection("ServiceInterval").Value;
        if (interval != null)
        {
            counter += int.Parse(interval);
        }
    }

    private void StartUmSetting()
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private void StartLogSetting()
    {
        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
        };

        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory(configuration.GetSection("SiteCategory").Value).Use(logEngine);
    }
}