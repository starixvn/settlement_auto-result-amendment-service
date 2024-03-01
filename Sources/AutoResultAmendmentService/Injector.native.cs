using AutoResultAmendment.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Transport;

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
}