using MassTransit;
using Microsoft.Extensions.Logging;
using ResultAmendment.Contracts;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.UpdateSettlementResultHistory;

namespace ResultAmendment.Consumers;

public sealed class ResultAmendedConsumer : IConsumer<ResultAmended>, IConsumer<ResultAmendSettled>
{
    private readonly ILogger<ResultAmendedConsumer> _logger;
    private readonly AddSettlementResultHistoryHandler addHandler;
    private readonly UpdateSettlementResultHistoryHandler updateHandler;

    public ResultAmendedConsumer(ILogger<ResultAmendedConsumer> logger, AddSettlementResultHistoryHandler addHandler, UpdateSettlementResultHistoryHandler updateHandler)
    {
        _logger = logger;
        this.addHandler = addHandler;
        this.updateHandler = updateHandler;
    }

    public async Task Consume(ConsumeContext<ResultAmended> context)
    {
        _logger.LogInformation("ResultAmendedConsumer: {0}", context.Message);

        await addHandler.HandleAsync(context.Message);
    }

    public async Task Consume(ConsumeContext<ResultAmendSettled> context)
    {
        _logger.LogInformation("ResultAmendSettled: {0}", context.Message);
        await updateHandler.HandlerAsync(context.Message);
    }
}