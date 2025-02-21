using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.Settlement.Converter.Services;
using ONEbook.UM.DependencyInjection;
using SettlementResultHistory.Services;
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
        services.AddOneBookUmForService(configuration);
        services.AddFanexLogSentry(configuration);

        ConfigureRedis(services, configuration);

        MailAppSettings.AppMailId = configuration.GetSection("MailServer:Id").Value;
        MailAppSettings.AppMailAddress = configuration.GetSection("MailServer:Address").Value;
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
}