using AutoResultAmendment.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Transport;
using SharpRaven;
using MailAppSettings = Fanex.MailDeliver.MailClient.AppSettings;

namespace AutoResultAmendmentService;

public static class Injector
{
    public static void InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<SettlementResultHistoryService, SettlementResultHistoryServiceImp>();
        services.AddSingleton<SettlementResultHistoryRepository, SettlementResultHistoryRepositoryImp>();
        services.AddTransient<IMaintenanceService, MaintenanceServiceImp>();
        services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();
        services.AddSingleton<ONEbook.UM.Services.IMaintenanceService, ONEbook.UM.Services.MaintenanceService>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        ConfigureRedis(services, configuration);

        StartUmSetting(configuration);
        StartLogSetting(configuration);
    }

    private static void ConfigureRedis(IServiceCollection service, IConfiguration configuration)
    {
        var redisConnection = configuration.GetValue("Redis:ConnectionString", "");
        var applicationName = configuration.GetValue("Redis:ApplicationName", "Settlement");

        service.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisConnection,
            ApplicationName = applicationName,
        }));
        service.AddSingleton<ICacheService>(new CacheService());
    }

    private static void StartUmSetting(IConfiguration configuration)
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private static void StartLogSetting(IConfiguration configuration)
    {
        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
        };

        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory(configuration.GetSection("SiteCategory").Value).Use(logEngine);

        MailAppSettings.AppMailId = configuration.GetSection("MailServer:Id").Value;
        MailAppSettings.AppMailAddress = configuration.GetSection("MailServer:Address").Value;
    }
}