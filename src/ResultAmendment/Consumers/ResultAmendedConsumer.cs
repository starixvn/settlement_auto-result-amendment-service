using Fanex.Logging;
using MassTransit;
using ResultAmendment.Contracts;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.UpdateSettlementResultHistory;

namespace ResultAmendment.Consumers;

public sealed class ResultAmendedConsumer : IConsumer<ResultAmended>, IConsumer<ResultAmendSettled>
{
    private readonly AddSettlementResultHistoryHandler addHandler;
    private readonly UpdateSettlementResultHistoryHandler updateHandler;

    public ResultAmendedConsumer(
        AddSettlementResultHistoryHandler addHandler,
        UpdateSettlementResultHistoryHandler updateHandler)
    {
        this.addHandler = addHandler;
        this.updateHandler = updateHandler;
    }

    public async Task Consume(ConsumeContext<ResultAmended> context)
    {
        await LogService.InfoAsync($"ResultAmendedConsumer Start {context.Message.MatchId} {context.Message.BroadcastSessionId}", context.Message);
        await addHandler.HandleAsync(context.Message);
        await LogService.InfoAsync($"ResultAmendedConsumer Done {context.Message.MatchId} {context.Message.BroadcastSessionId}", context.Message);
    }

    public async Task Consume(ConsumeContext<ResultAmendSettled> context)
    {
        await LogService.InfoAsync($"ResultAmendSettled Start {DateTime.Now} {context.Message.MatchId}", context.Message);
        await updateHandler.HandleAsync(context.Message);
        await LogService.InfoAsync($"ResultAmendSettled Done {DateTime.Now} {context.Message.MatchId}", context.Message);
    }
}