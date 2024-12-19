using AutoResultAmendment.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.Settlement.Converter.Services;
using ONEbook.UM.DependencyInjection;
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
        services.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        services.AddOneBookUm(configuration);

        ConfigureRedis(services, configuration);

        StartLogSetting(configuration);
    }

    private static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetValue("Redis:ConnectionString", "");
        var applicationName = configuration.GetValue("Redis:ApplicationName", "Settlement");

        services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisConnection,
            ApplicationName = applicationName,
        }));
        services.AddSingleton<ICacheService>(new CacheService());
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