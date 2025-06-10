using Fanex.Settlement.MessageBridge.ResultAmendment;
using MassTransit;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.UpdateSettlementResultHistory;

namespace ResultAmendment.Consumers;

public sealed class ResultAmendedConsumer(
    AddSettlementResultHistoryHandler addHandler,
    UpdateSettlementResultHistoryHandler updateHandler)
    : IConsumer<ResultAmended>, IConsumer<ResultAmendSettled>
{
    public async Task Consume(ConsumeContext<ResultAmended> context)
    {
        await addHandler.HandleAsync(context.Message);
    }

    public async Task Consume(ConsumeContext<ResultAmendSettled> context)
    {
        await updateHandler.HandleAsync(context.Message);
    }
}