using Microsoft.Extensions.DependencyInjection;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class SetResultsStrategyExtensions
{
    public static IServiceCollection AddSetResultsStrategy(this IServiceCollection services)
    {
        services.AddScoped<ISetResultStrategy, MainResultStrategy>();
        services.AddScoped<ISetResultStrategy, TeamResultStrategy>();
        services.AddScoped<ISetResultStrategy, TeamXXResultStrategy>();
        services.AddScoped<ISetResultStrategy, TeamXXYYResultStrategy>();
        services.AddScoped<ISetResultStrategy, MapXResultStrategy>();
        services.AddScoped<ISetResultStrategy, MapXYResultStrategy>();
        services.AddScoped<ISetResultStrategy, MapXLaneResultStrategy>();
        services.AddScoped<ISetResultStrategy, MapXOverlordTyrantResultStrategy>();
        services.AddScoped<ISetResultStrategy, TimeKillsResultStrategy>();
        services.AddScoped<ISetResultStrategy, TimeResultStrategy>();
        services.AddScoped<ISetResultStrategy, MapXYesNoResultStrategy>();

        return services;
    }
}
