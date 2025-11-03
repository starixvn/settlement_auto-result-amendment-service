using Fanex.Logging;
using Fanex.Settlement.MessageBridge.ResultAmendment;
using MassTransit;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.UpdateSettlementResultHistory;
using System.Text.Json;

namespace ResultAmendment.Consumers;

public sealed class ResultAmendedConsumer(
    AddSettlementResultHistoryHandler addHandler,
    UpdateSettlementResultHistoryHandler updateHandler)
    : IConsumer<ResultAmended>, IConsumer<ResultAmendSettled>
{
    public async Task Consume(ConsumeContext<ResultAmended> context)
    {
        await LogService.InfoAsync($"ResultAmendedConsumer Start {DateTimeOffset.Now}: {JsonSerializer.Serialize(context.Message)}");
        await addHandler.HandleAsync(context.Message);
        await LogService.InfoAsync($"ResultAmendedConsumer Done {DateTimeOffset.Now}:  {JsonSerializer.Serialize(context.Message)}");
    }

    public async Task Consume(ConsumeContext<ResultAmendSettled> context)
    {
        await LogService.InfoAsync($"ResultAmendSettled Start {DateTimeOffset.Now}: {JsonSerializer.Serialize(context.Message)}");
        await updateHandler.HandleAsync(context.Message);
        await LogService.InfoAsync($"ResultAmendSettled Done {DateTimeOffset.Now}: {JsonSerializer.Serialize(context.Message)}");
    }
}