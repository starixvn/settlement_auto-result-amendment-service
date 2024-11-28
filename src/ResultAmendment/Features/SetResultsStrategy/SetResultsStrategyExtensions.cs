using Microsoft.Extensions.DependencyInjection;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class SetResultsStrategyExtensions
{
    public static IServiceCollection AddSetResultsStrategy(this IServiceCollection services)
    {
        services.AddSingleton<ISetResultStrategy, MainResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TeamResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TeamXXResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TeamXXYYResultStrategy>();
        services.AddSingleton<ISetResultStrategy, MapXResultStrategy>();
        services.AddSingleton<ISetResultStrategy, MapXYResultStrategy>();
        services.AddSingleton<ISetResultStrategy, MapXLaneResultStrategy>();
        services.AddSingleton<ISetResultStrategy, MapXOverlordTyrantResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TimeKillsResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TimeResultStrategy>();
        services.AddSingleton<ISetResultStrategy, TeamXYOrdinalResultStrategy>();
        services.AddSingleton<ISetResultStrategy, MapXYesNoResultStrategy>();

        return services;
    }
}
