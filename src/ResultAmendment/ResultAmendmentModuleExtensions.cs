using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ResultAmendment.Consumers;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.GetMainResource;

namespace ResultAmendment;

public static class ResultAmendmentModuleExtensions
{
    public static WebApplicationBuilder AddResultAmendmentModule(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddMemoryCache()
            .AddScoped<SettlementResultHistoryHandler>()
            .AddScoped<GetMainResourceHandler>()
            .AddMapster();

        return builder;
    }
    public static void AddResultAmendConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ResultAmendedConsumer>();
    }
}
